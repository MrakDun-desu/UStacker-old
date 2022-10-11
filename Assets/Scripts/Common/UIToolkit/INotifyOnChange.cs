using System;

namespace Blockstacker.Common.UIToolkit
{
    public interface INotifyOnChange
    {
        public event Action Changed;
    }
}