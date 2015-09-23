﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using ZWave.Driver.Communication;

namespace ZWave.Driver.CommandClasses
{
    public class SensorMultiLevelReport : NodeReport
    {
        public readonly SensorType Type;
        public readonly float Value;
        public readonly byte Scale;

        internal SensorMultiLevelReport(Node node, byte[] payload) : base(node)
        {
            Type = (SensorType)payload[0];
            Value = ParseValue(payload.Skip(1).ToArray(), out Scale);
        }

        private float ParseValue(byte[] payload, out byte scale)
        {
            // bits 7,6,5: precision, bits 4,3: scale, bits 2,1,0 : size
            var precision = (byte)((payload[0] & 0xE0) >> 5);
            scale = (byte)((payload[0] & 0x18) >> 3);
            var size = (byte)(payload[0] & 0x07);

            var value = (long)payload[1];
            switch (size)
            {
                case 2:
                    value = PayloadConverter.ToInt16(payload, 1);
                    break;
                case 4:
                    value = PayloadConverter.ToInt32(payload, 1);
                    break;
                case 8:
                    value = PayloadConverter.ToInt64(payload, 1);
                    break;
            }
            return (float)(value / Math.Pow(10, precision));
        }

        public override string ToString()
        {
            return $"Type:{Type}, Value:{Value}, Scale:{Scale}";
        }
    }
}