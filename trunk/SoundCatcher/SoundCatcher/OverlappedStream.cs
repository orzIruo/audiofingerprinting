using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;

namespace SoundCatcher
{
    public class OverlappedStream : Stream
    {
        private const int BlockSize = 65536;
        private const int MaxBlocksInCache = (3 * 1024 * 1024) / BlockSize;

        private int m_Size;
        private int m_RPos;
        private int m_WPos;
        private Stack m_UsedBlocks = new Stack();
        private ArrayList m_Blocks = new ArrayList();

        private byte[] AllocBlock()
        {
            byte[] Result = null;
            Result = m_UsedBlocks.Count > 0 ? (byte[])m_UsedBlocks.Pop() : new byte[BlockSize];
            return Result;
        }
        private void FreeBlock(byte[] block)
        {
            if (m_UsedBlocks.Count < MaxBlocksInCache)
                m_UsedBlocks.Push(block);
        }
        private byte[] GetWBlock()
        {
            byte[] Result = null;
            if (m_WPos < BlockSize && m_Blocks.Count > 0)
                Result = (byte[])m_Blocks[m_Blocks.Count - 1];
            else
            {
                Result = AllocBlock();
                m_Blocks.Add(Result);
                m_WPos = 0;
            }
            return Result;
        }

        // Stream members
        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override void Flush()
        {
            throw new NotImplementedException();
        }

        public override long Length
        {
            get { throw new NotImplementedException(); }
        }

        public override long Position
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            lock (this)
            {
                int Left = count;
                while (Left > 0)
                {
                    int ToWrite = Math.Min(BlockSize - m_WPos, Left);
                    Array.Copy(buffer, offset + count - Left, GetWBlock(), m_WPos, ToWrite);
                    m_WPos += ToWrite;
                    Left -= ToWrite;
                }
                m_Size += count;
            }
        }
    }
}
