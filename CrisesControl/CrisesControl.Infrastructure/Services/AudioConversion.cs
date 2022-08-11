using NAudio.MediaFoundation;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Infrastructure.Services
{
    public static class AudioConversion
    {
        public delegate void CatchException(Exception ex);
        /// <summary>
        /// Convert input audio file to MP3.
        /// Saves generated MP3 in the same location and with the same name.
        /// This will overwrite the output file if already exists.
        /// </summary>
        /// <param name="filePath">audio file to convert to MP3.</param>
        /// <param name="catchException">function to catch exceptions that may occur during conversion. can be set to null.
        /// Will NOT catch exceptions due to MFTs not being available.</param>
        /// <returns>output file path; null if failed.</returns>
        public static string ToMp3(string filePath, CatchException catchException)
        {
            // check if input file exists.
            if (File.Exists(filePath))
            {
                // compute the output file
                string outfile = Path.Combine(Path.GetDirectoryName(filePath), $"{Path.GetFileNameWithoutExtension(filePath)}.mp3");
                // convert input to mp3.
                using (var audioFile = new AudioFileReader(filePath))
                {
                    bool success = true;
                    MediaFoundationApi.Startup();
                    try
                    {
                        MediaFoundationEncoder.EncodeToMp3(audioFile, outfile, desiredBitRate: 128000);
                    }
                    catch (Exception ex)
                    {
                        //Debug.WriteLine(ex.Message);
                        success = false;
                        if (catchException != null)
                            catchException(ex);
                    }
                    finally
                    {
                        MediaFoundationApi.Shutdown();
                    }
                    return success ? outfile : null;
                }
            }
            return null;
        }
    }
}
