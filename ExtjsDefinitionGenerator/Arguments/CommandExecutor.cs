using System;
using System.Diagnostics;

namespace ExtjsDefinitionGenerator.Arguments
{
    public static class CommandExecutor
    {
        public static void ExecuteCommand(string tool, string command)
        {
            try
            {

                var proc = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = tool,
                        Arguments = command,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    }
                };

                proc.Start();
                proc.WaitForExit();
            }
            catch (Exception ex)
            {

            }
        }
    }
}
