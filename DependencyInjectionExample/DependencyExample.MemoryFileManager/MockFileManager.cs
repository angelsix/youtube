using System;

namespace DependencyExample
{
    public class MockFileManager : IFileManager
    {
        private string mData;

        public bool FailOnPurpose { get; set; }

        public string GetFileData(string path)
        {
            return mData;
        }

        public void WriteFileData(string path, string data)
        {
            DependencyProvider.Logger.LogMessage($"Mocking `{data}` to `{path}`, appending ` mock` to the end");

            if (FailOnPurpose)
                mData = $"{data} mock";
            else
                mData = data;
        }
    }
}
