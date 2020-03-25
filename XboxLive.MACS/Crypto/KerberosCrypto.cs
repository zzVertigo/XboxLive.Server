using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace XboxLive.MACS.Crypto
{
    public class KerberosCrypto
    {
        public static void ComputeAllKerberosPasswordHashes(string password, string userName = "",
            string domainName = "")
        {
            // use KerberosPasswordHash() to calculate rc4_hmac, aes128_cts_hmac_sha1, aes256_cts_hmac_sha1, and des_cbc_md5 hashes for a given password

            Console.WriteLine("\r\n[*] Action: Calculate Password Hashes\r\n");

            Console.WriteLine("[*] Input password             : {0}", password);

            var salt = string.Format("{0}{1}", domainName.ToUpper(), userName.ToLower());

            if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(domainName))
            {
                Console.WriteLine("[*] Input username             : {0}", userName);
                Console.WriteLine("[*] Input domain               : {0}", domainName);
                Console.WriteLine("[*] Salt                       : {0}", salt);
            }

            var rc4Hash = KerberosPasswordHash(Interop.KERB_ETYPE.rc4_hmac, password);
            Console.WriteLine("[*]       rc4_hmac             : {0}", rc4Hash);

            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(domainName))
            {
                Console.WriteLine(
                    "\r\n[!] /user:X and /domain:Y need to be supplied to calculate AES and DES hash types!");
            }
            else
            {
                var aes128Hash = KerberosPasswordHash(Interop.KERB_ETYPE.aes128_cts_hmac_sha1, password, salt);
                Console.WriteLine("[*]       aes128_cts_hmac_sha1 : {0}", aes128Hash);

                var aes256Hash = KerberosPasswordHash(Interop.KERB_ETYPE.aes256_cts_hmac_sha1, password, salt);
                Console.WriteLine("[*]       aes256_cts_hmac_sha1 : {0}", aes256Hash);

                var desHash = KerberosPasswordHash(Interop.KERB_ETYPE.des_cbc_md5,
                    string.Format("{0}{1}", password, salt), salt);
                Console.WriteLine("[*]       des_cbc_md5          : {0}", desHash);
            }

            Console.WriteLine();
        }

        public static string KerberosPasswordHash(Interop.KERB_ETYPE etype, string password, string salt = "",
            int count = 4096)
        {
            // use the internal KERB_ECRYPT HashPassword() function to calculate a password hash of a given etype
            // adapted from @gentilkiwi's Mimikatz "kerberos::hash" implementation

            Interop.KERB_ECRYPT pCSystem;
            IntPtr pCSystemPtr;

            // locate the crypto system for the hash type we want
            var status = Interop.CDLocateCSystem(etype, out pCSystemPtr);

            pCSystem = (Interop.KERB_ECRYPT) Marshal.PtrToStructure(pCSystemPtr, typeof(Interop.KERB_ECRYPT));
            if (status != 0)
                throw new Win32Exception(status, "Error on CDLocateCSystem");

            // get the delegate for the password hash function
            var pCSystemHashPassword =
                (Interop.KERB_ECRYPT_HashPassword) Marshal.GetDelegateForFunctionPointer(pCSystem.HashPassword,
                    typeof(Interop.KERB_ECRYPT_HashPassword));
            var passwordUnicode = new Interop.UNICODE_STRING(password);
            var saltUnicode = new Interop.UNICODE_STRING(salt);

            var output = new byte[pCSystem.KeySize];

            var success = pCSystemHashPassword(passwordUnicode, saltUnicode, count, output);

            if (status != 0)
                throw new Win32Exception(status);

            return BitConverter.ToString(output).Replace("-", "");
        }

        // Adapted from Vincent LE TOUX' "MakeMeEnterpriseAdmin"
        public static byte[] KerberosChecksum(byte[] key, byte[] data)
        {
            Interop.KERB_CHECKSUM pCheckSum;
            IntPtr pCheckSumPtr;
            var status = Interop.CDLocateCheckSum(Interop.KERB_CHECKSUM_ALGORITHM.KERB_CHECKSUM_HMAC_MD5,
                out pCheckSumPtr);
            pCheckSum = (Interop.KERB_CHECKSUM) Marshal.PtrToStructure(pCheckSumPtr, typeof(Interop.KERB_CHECKSUM));
            if (status != 0) throw new Win32Exception(status, "CDLocateCheckSum failed");

            IntPtr Context;
            var pCheckSumInitializeEx =
                (Interop.KERB_CHECKSUM_InitializeEx) Marshal.GetDelegateForFunctionPointer(pCheckSum.InitializeEx,
                    typeof(Interop.KERB_CHECKSUM_InitializeEx));
            var pCheckSumSum =
                (Interop.KERB_CHECKSUM_Sum) Marshal.GetDelegateForFunctionPointer(pCheckSum.Sum,
                    typeof(Interop.KERB_CHECKSUM_Sum));
            var pCheckSumFinalize =
                (Interop.KERB_CHECKSUM_Finalize) Marshal.GetDelegateForFunctionPointer(pCheckSum.Finalize,
                    typeof(Interop.KERB_CHECKSUM_Finalize));
            var pCheckSumFinish =
                (Interop.KERB_CHECKSUM_Finish) Marshal.GetDelegateForFunctionPointer(pCheckSum.Finish,
                    typeof(Interop.KERB_CHECKSUM_Finish));

            // initialize the checksum
            // KERB_NON_KERB_CKSUM_SALT = 17
            var status2 = pCheckSumInitializeEx(key, key.Length, 17, out Context);
            if (status2 != 0)
                throw new Win32Exception(status2);

            // the output buffer for the checksum data
            var checksumSrv = new byte[pCheckSum.Size];

            // actually checksum all the supplied data
            pCheckSumSum(Context, data.Length, data);

            // finish everything up
            pCheckSumFinalize(Context, checksumSrv);
            pCheckSumFinish(ref Context);

            return checksumSrv;
        }

        // Adapted from Vincent LE TOUX' "MakeMeEnterpriseAdmin"
        //  https://github.com/vletoux/MakeMeEnterpriseAdmin/blob/master/MakeMeEnterpriseAdmin.ps1#L2235-L2262
        public static byte[] KerberosDecrypt(Interop.KERB_ETYPE eType, int keyUsage, byte[] key, byte[] data)
        {
            Interop.KERB_ECRYPT pCSystem;
            IntPtr pCSystemPtr;

            // locate the crypto system
            var status = Interop.CDLocateCSystem(eType, out pCSystemPtr);
            pCSystem = (Interop.KERB_ECRYPT) Marshal.PtrToStructure(pCSystemPtr, typeof(Interop.KERB_ECRYPT));
            if (status != 0)
                throw new Win32Exception(status, "Error on CDLocateCSystem");

            // initialize everything
            IntPtr pContext;
            var pCSystemInitialize =
                (Interop.KERB_ECRYPT_Initialize) Marshal.GetDelegateForFunctionPointer(pCSystem.Initialize,
                    typeof(Interop.KERB_ECRYPT_Initialize));
            var pCSystemDecrypt =
                (Interop.KERB_ECRYPT_Decrypt) Marshal.GetDelegateForFunctionPointer(pCSystem.Decrypt,
                    typeof(Interop.KERB_ECRYPT_Decrypt));
            var pCSystemFinish =
                (Interop.KERB_ECRYPT_Finish) Marshal.GetDelegateForFunctionPointer(pCSystem.Finish,
                    typeof(Interop.KERB_ECRYPT_Finish));
            status = pCSystemInitialize(key, key.Length, keyUsage, out pContext);
            if (status != 0)
                throw new Win32Exception(status);

            var outputSize = data.Length;
            if (data.Length % pCSystem.BlockSize != 0)
                outputSize += pCSystem.BlockSize - data.Length % pCSystem.BlockSize;

            var algName = Marshal.PtrToStringAuto(pCSystem.AlgName);

            outputSize += pCSystem.Size;
            var output = new byte[outputSize];

            // actually perform the decryption
            status = pCSystemDecrypt(pContext, data, data.Length, output, ref outputSize);
            pCSystemFinish(ref pContext);

            return output;
        }

        // Adapted from Vincent LE TOUX' "MakeMeEnterpriseAdmin"
        //  https://github.com/vletoux/MakeMeEnterpriseAdmin/blob/master/MakeMeEnterpriseAdmin.ps1#L2235-L2262
        public static byte[] KerberosEncrypt(Interop.KERB_ETYPE eType, int keyUsage, byte[] key, byte[] data)
        {
            Interop.KERB_ECRYPT pCSystem;
            IntPtr pCSystemPtr;

            // locate the crypto system
            var status = Interop.CDLocateCSystem(eType, out pCSystemPtr);
            pCSystem = (Interop.KERB_ECRYPT) Marshal.PtrToStructure(pCSystemPtr, typeof(Interop.KERB_ECRYPT));
            if (status != 0)
                throw new Win32Exception(status, "Error on CDLocateCSystem");

            // initialize everything
            IntPtr pContext;
            var pCSystemInitialize =
                (Interop.KERB_ECRYPT_Initialize) Marshal.GetDelegateForFunctionPointer(pCSystem.Initialize,
                    typeof(Interop.KERB_ECRYPT_Initialize));
            var pCSystemEncrypt =
                (Interop.KERB_ECRYPT_Encrypt) Marshal.GetDelegateForFunctionPointer(pCSystem.Encrypt,
                    typeof(Interop.KERB_ECRYPT_Encrypt));
            var pCSystemFinish =
                (Interop.KERB_ECRYPT_Finish) Marshal.GetDelegateForFunctionPointer(pCSystem.Finish,
                    typeof(Interop.KERB_ECRYPT_Finish));
            status = pCSystemInitialize(key, key.Length, keyUsage, out pContext);
            if (status != 0)
                throw new Win32Exception(status);

            var outputSize = data.Length;
            if (data.Length % pCSystem.BlockSize != 0)
                outputSize += pCSystem.BlockSize - data.Length % pCSystem.BlockSize;

            var algName = Marshal.PtrToStringAuto(pCSystem.AlgName);

            outputSize += pCSystem.Size;
            var output = new byte[outputSize];

            // actually perform the decryption
            status = pCSystemEncrypt(pContext, data, data.Length, output, ref outputSize);
            pCSystemFinish(ref pContext);

            return output;
        }
    }
}