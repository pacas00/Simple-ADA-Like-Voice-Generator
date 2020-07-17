using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Google.Cloud.TextToSpeech.V1;

namespace ADA_AudioGenWin
{
public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (Directory.Exists("AUTH"))
            {
                foreach (var file in Directory.EnumerateFiles("AUTH\\"))
                {
                    if (file.ToLower().EndsWith(".json"))
                    {
                        Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", file);
                        break;
                    }
                }
                if (Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS") == null)
                {
                    MessageBox.Show("No JSON file in AUTH folder. Please put the JSON file in the AUTH folder");
                    return;
                }

            } else
            {
                Directory.CreateDirectory("AUTH");
                MessageBox.Show("Please put the JSON file in the AUTH folder");
                return;
            }
            
            // Instantiate a client
            TextToSpeechClient client = TextToSpeechClient.Create();

            // Set the text input to be synthesized.
            SynthesisInput input = new SynthesisInput
            {
                Text = this.textBox1.Text
            };

            // Build the voice request, select the language code ("en-US"),
            // and the SSML voice gender ("neutral").
            VoiceSelectionParams voice = new VoiceSelectionParams
            {
                LanguageCode = "en-US",
                SsmlGender = SsmlVoiceGender.Female,
                Name = "en-US-Standard-C"

            };

            // Select the type of audio file you want returned.
            AudioConfig config = new AudioConfig
            {
                AudioEncoding = AudioEncoding.Mp3
            };

            // Perform the Text-to-Speech request, passing the text input
            // with the selected voice parameters and audio file type
            var response = client.SynthesizeSpeech(new SynthesizeSpeechRequest
            {
                Input = input,
                Voice = voice,
                AudioConfig = config
            });

            
            //Configure Save Dialog
            this.saveFileDialog1.Filter = "MP3 Audio | *.mp3";
            this.saveFileDialog1.DefaultExt = "mp3";
            this.saveFileDialog1.FileName = "output.mp3";
            this.saveFileDialog1.InitialDirectory = ".\\";
            
            DialogResult result = this.saveFileDialog1.ShowDialog();

            if (result != DialogResult.OK)
            {
                MessageBox.Show("Generation Aborted! No where to save.");
                return;
            }

            // Write the binary AudioContent of the response to an MP3 file.
            using (Stream output = saveFileDialog1.OpenFile())
            {
                response.AudioContent.WriteTo(output);
                Console.WriteLine($"Audio content written to file 'sample.mp3'");
            }
        }


    }
}
