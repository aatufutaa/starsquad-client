namespace StarSquad.Net.Encryption
{
    public class Rc4Encryption
    {
        public readonly byte[] secret;

        public readonly Rc4 encrypt;
        public readonly Rc4 decrypt;

        public Rc4Encryption()
        {
            this.secret = new byte[16];
            new System.Random().NextBytes(this.secret);

            this.encrypt = new Rc4(this.secret);
            this.decrypt = new Rc4(this.secret);
        }

        public class Rc4
        {
            private const int SBOX_LENGTH = 256;

            private readonly byte[] key;
            private readonly int[] sbox;

            public Rc4(byte[] key)
            {
                this.key = key;
                this.sbox = new int[SBOX_LENGTH];
            }

            public void Init()
            {
                int j = 0;

                for (int i = 0; i < SBOX_LENGTH; i++)
                {
                    this.sbox[i] = i;
                }

                for (int i = 0; i < SBOX_LENGTH; i++)
                {
                    j = (j + this.sbox[i] + (this.key[i % this.key.Length]) & 0xFF) % SBOX_LENGTH;
                    this.Swap(i, j, this.sbox);
                }
            }

            public void Crypt(byte[] inBuf, int inOffset, byte[] outBuf, int outOffset, int length)
            {
                int i = 0;
                int j = 0;
                for (int n = 0; n < length; n++)
                {
                    i = (i + 1) % SBOX_LENGTH;
                    j = (j + this.sbox[i]) % SBOX_LENGTH;
                    this.Swap(i, j, this.sbox);
                    int rand = this.sbox[(this.sbox[i] + this.sbox[j]) % SBOX_LENGTH];
                    outBuf[outOffset + n] = (byte)(rand ^ inBuf[inOffset + n]);
                }
            }

            private void Swap(int i, int j, int[] sbox)
            {
                int temp = sbox[i];
                sbox[i] = sbox[j];
                sbox[j] = temp;
            }
        }
    }
}