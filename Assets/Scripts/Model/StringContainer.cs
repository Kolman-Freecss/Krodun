using Unity.Netcode;

namespace Model
{
    public class StringContainer : NetworkVariableBase
    {
        /// Managed list of class instances
    public string Text = default;

    /// <summary>
    /// Writes the complete state of the variable to the writer
    /// </summary>
    /// <param name="writer">The stream to write the state to</param>
    public override void WriteField(FastBufferWriter writer)
    {
        // If there is nothing, then return 0 as the string size
        if (string.IsNullOrEmpty(Text))
        {
            writer.WriteValueSafe(0);
            return;
        }

        var textByteArray = System.Text.Encoding.ASCII.GetBytes(Text);

        // Write the total size of the string
        writer.WriteValueSafe(textByteArray.Length);
        var toalBytesWritten = 0;
        var bytesRemaining = textByteArray.Length;
        // Write the string values
        while (bytesRemaining > 0)
        {
            writer.WriteValueSafe(textByteArray[toalBytesWritten]);
            toalBytesWritten++;
            bytesRemaining = textByteArray.Length - toalBytesWritten;
        }
    }

    /// <summary>
    /// Reads the complete state from the reader and applies it
    /// </summary>
    /// <param name="reader">The stream to read the state from</param>
    public override void ReadField(FastBufferReader reader)
    {
        // Reset our string to empty
        Text = string.Empty;
        var stringSize = (int)0;
        // Get the string size in bytes
        reader.ReadValueSafe(out stringSize);

        // If there is nothing, then we are done
        if (stringSize == 0)
        {
            return;
        }

        // allocate an byte array to 
        var byteArray = new byte[stringSize];
        var tempByte = (byte)0;
        for(int i = 0; i < stringSize; i++)
        {
            reader.ReadValueSafe(out tempByte);
            byteArray[i] = tempByte;
        }
        
        // Convert it back to a string
        Text = System.Text.Encoding.ASCII.GetString(byteArray);
    }

    public override void ReadDelta(FastBufferReader reader, bool keepDirtyDelta)
    {
        // Do nothing for this example
    }

    public override void WriteDelta(FastBufferWriter writer)
    {
        // Do nothing for this example
    }
    }
}