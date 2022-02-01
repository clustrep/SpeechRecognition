using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.IO;
using System.Globalization;

namespace SpeakingWithFer
{
    public partial class Form1 : Form
    {
        //private static CultureInfo _speech = new CultureInfo("es-PE", false);
        //private static SpeechRecognitionEngine _recognizer = new SpeechRecognitionEngine(new CultureInfo("ar-SA"));
        ////CultureInfo aqweqw = new CultureInfo("es - PE");
        SpeechRecognitionEngine _recognizer = new SpeechRecognitionEngine();
        SpeechSynthesizer Fer = new SpeechSynthesizer();
        SpeechRecognitionEngine startlistening = new SpeechRecognitionEngine();
        Random rnd = new Random();
        int RecTimeOut = 0;
        DateTime TimeNow = DateTime.Now;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _recognizer.SetInputToDefaultAudioDevice();
            _recognizer.LoadGrammarAsync(new Grammar(new GrammarBuilder(new Choices(File.ReadAllLines(@"DefaultCommands.txt")))));
            _recognizer.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(Default_SpeechRecognized);
            _recognizer.SpeechDetected += new EventHandler<SpeechDetectedEventArgs>(_recognizer_SpeechRecognized);
            _recognizer.RecognizeAsync(RecognizeMode.Multiple);

            startlistening.SetInputToDefaultAudioDevice();
            startlistening.LoadGrammarAsync(new Grammar(new GrammarBuilder(new Choices(File.ReadAllLines(@"DefaultCommands.txt")))));
            startlistening.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(startlistening_SpeechRecognized);
        }

        private void Default_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            int ranNum;
            string speech = e.Result.Text;

            if(speech == "Hello Fer")
            {
                Fer.SpeakAsync("Hola, ya estoy aquí");
            }
            if (speech == "how are you")
            {
                Fer.SpeakAsync("Estoy bien, espero que tu también");
            }
            if (speech == "what time is it")
            {
                Fer.SpeakAsync(TimeNow.ToString("h mm tt"));
            }
            if(speech == "Fer no hables")
            {
                Fer.SpeakAsyncCancelAll();
                ranNum = rnd.Next(1);
                if(ranNum == 1)
                {
                    Fer.SpeakAsync("Oka");
                }
                if(ranNum == 2)
                {
                    Fer.SpeakAsync("Estaré callada");
                }
            }
            if(speech == "Don't listen")
            {
                Fer.SpeakAsync("Si necesitas algo solo hablame");
                _recognizer.RecognizeAsyncCancel();
                startlistening.RecognizeAsync(RecognizeMode.Multiple);
            }

            if(speech == "Show Commands")
            {
                string[] commands = (File.ReadAllLines(@"DefaultCommands.txt"));
                LstCommands.Items.Clear();
                LstCommands.SelectionMode = SelectionMode.None;
                LstCommands.Visible = true;

                foreach(string command in commands)
                {
                    LstCommands.Items.Add(command);
                }
            }
            if(speech == "Oculta comandos")
            {
                LstCommands.Visible = false;
            }
        }

        private void _recognizer_SpeechRecognized(object sender, SpeechDetectedEventArgs e)
        {
            RecTimeOut = 0;
        }

        private void startlistening_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            string speech = e.Result.Text;

            if(speech == "Fer estas ahí")
            {
                startlistening.RecognizeAsyncCancel();
                Fer.SpeakAsync("Si, estoy aquí");
                _recognizer.RecognizeAsync(RecognizeMode.Multiple);
            }
        }

        private void TmrSpeaking_Tick(object sender, EventArgs e)
        {
            if(RecTimeOut == 10)
            {
                _recognizer.RecognizeAsyncCancel();
            }
            else if(RecTimeOut == 11)
            {
                TmrSpeaking.Stop();
                startlistening.RecognizeAsync(RecognizeMode.Multiple);
                RecTimeOut = 0;
            }
        }
    }
}
