﻿using Parchive.Library.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parchive.Library.PAR2.Packets
{
    /// <summary>
    /// A PAR2 file description packet.
    /// </summary>
    [Packet(0x00302E3220524150, 0x63736544656C6946)]
    public class FileDescriptionPacket : Packet
    {
        #region Properties
        /// <summary>
        /// The File ID is calculated as the MD5 Hash of the last 3 fields of the body of this packet.
        /// </summary>
        public FileID FileID { get; set; } = new FileID();

        /// <summary>
        /// The MD5 hash of the entire file.
        /// </summary>
        public byte[] Hash { get; set; } = new byte[16];

        /// <summary>
        /// The MD5-16k. That is, the MD5 hash of the first 16kB of the file.
        /// </summary>
        public byte[] Hash16k { get; set; } = new byte[16];

        /// <summary>
        /// The length of the entire file.
        /// </summary>
        public long Length { get; set; } = 0;

        /// <summary>
        /// Name of the file.
        /// Subdirectories are indicated by an HTML-style '/' (a.k.a. the UNIX slash).
        /// The filename must be unique.
        /// </summary>
        public string Filename { get; set; } = string.Empty;

        /// <summary>
        /// The packet body in the form of a <see cref="Stream"/> object.
        /// </summary>
        public override Stream Body
        {
            get
            {
                throw new NotImplementedException();
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initializes the packet from a stream through a <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">A <see cref="Stream"/> containing the packet.</param>
        protected override void Initialize(Stream stream)
        {
            using (var reader = new BinaryReader(stream, Encoding.ASCII, true))
            {
                FileID = new FileID { ID = reader.ReadBytes(16) };
                Hash = reader.ReadBytes(16);
                Hash16k = reader.ReadBytes(16);

                if ((Length = reader.ReadInt64()) < 0)
                {
                    throw new TooLargeNumberError();
                }

                StringBuilder sb = new StringBuilder();

                while (reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    sb.Append(reader.ReadChars(4));
                }

                Filename = sb.ToString();
            }
        }
        #endregion
    }
}
