using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ArticleReader
{
    public class BaseArticle : Panel
    {
        public int Counter = 0;
        public SpeechSynthesizer Synth = new SpeechSynthesizer();
        public int LabelCount = 0;
        public int[] Counts = new int[1000];
        public Color BackColorA = Color.White;
        public string ReadText = "";
        public int Speed = 0;
        int PreviousSpeed = 0;
        public BaseArticle()
        {
            Synth.SelectVoice("Microsoft Zira Desktop");
            Synth.SpeakProgress += Synth_SpeakProgress;
            Synth.SpeakCompleted += Synth_SpeakCompleted;
            Synth.Rate = Speed;
        }

        

        void Synth_SpeakCompleted(object sender, SpeakCompletedEventArgs e)
        {
            VerticalScroll.Value = VerticalScroll.Maximum;
        }
        string LastWord = "";
        Boolean First = true;
        Boolean Switched = false;
        int Calls = 0;
        void Synth_SpeakProgress(object sender, SpeakProgressEventArgs e)
        {
            try
            {
                if (Speed != PreviousSpeed)
                {

                    string test = ReadText.Substring(Counter);
                    PreviousSpeed = Speed;
                    Synth.SpeakAsyncCancelAll();
                    Synth.Rate = Speed;
                    Switched = true;
                    Synth.SpeakAsync(test);
                }
                if (!Switched && LastWord != e.Text && (e.Text.Length != 1 || char.IsLetter(Convert.ToChar(e.Text))))
                {
                    
                    int temp = Counter;
                    LastWord = e.Text;
                    RichTextBox a = BoxByWordCount(Counter);
                    int SpecificWord = ConfineWord(Counter);
                    if (SpecificWord == 1) { SpecificWord = 0; }
                    string tempS = a.Text.Substring(SpecificWord);
                    int Highlight = tempS.IndexOf(e.Text) + SpecificWord + e.Text.Length;

                    Counter = TotalEndLine + Highlight + 1;
                    a.Select(0, Highlight);

                    a.SelectionColor = Color.Red;
                    ScrollControlIntoView(a);
                }
                else if (Switched)
                {
                    if (Calls == 5)
                    {
                        Switched = false;
                        Calls = 0;
                    }
                    else
                    {
                        RichTextBox a = BoxByWordCount(Counter);
                        ScrollControlIntoView(a);
                        Calls++;
                    }
                    
                }
            }
            catch (Exception e1)
            {

            }
        }
        public int ConfineWord(int num)
        {
            int LapCount = 0;
            int NumMinus = num;
            foreach (int i in Counts)
            {
                NumMinus -= i;
                if (NumMinus < 1)
                {
                    return NumMinus + i;
                }
            }
            return 0;
        }
        int EndedLine = 0;
        int TotalEndLine = 0;
        public RichTextBox BoxByWordCount(int num)
        {
            int LapCount = 0;
            int NumMinus = num;
            TotalEndLine = 0;
            foreach (int i in Counts)
            {
                TotalEndLine += i;
                NumMinus -= i;
                if (NumMinus < 1)
                {
                    foreach (RichTextBox a in Controls.OfType<RichTextBox>())
                    {
                        if (LapCount == 0)
                        {
                            EndedLine = i;
                            TotalEndLine -= i;
                            return a;
                        }
                        else
                        {
                            LapCount--;
                        }
                    }
                }
                else
                {
                    LapCount++;
                }
            }
            return new RichTextBox();
        }
        public string RemoveHashCode(string RemoveString)
        {
            // '
            while (RemoveString.Contains("&#8217;") || RemoveString.Contains("&#8216;") || RemoveString.Contains("â€™"))
            {
                string[] Split = RemoveString.Split(new string[] { "&#8217;", "&#8216;", "â€™" }, StringSplitOptions.RemoveEmptyEntries);
                RemoveString = "";
                for (int i = 0; i < Split.Length; i++)
                {
                    RemoveString += Split[i];
                    if (i != Split.Length - 1)
                    {
                        RemoveString += "'";
                    }
                }
            }
            // "
            while (RemoveString.Contains("&#8220;") || RemoveString.Contains("&#8221;"))
            {
                string[] Split = RemoveString.Split(new string[] { "&#8220;", "&#8221;" }, StringSplitOptions.RemoveEmptyEntries);
                RemoveString = "";
                for (int i = 0; i < Split.Length; i++)
                {
                    RemoveString += Split[i];
                    if (i != Split.Length - 1)
                    {
                        RemoveString += "\"";
                    }
                }
            }

            // -
            while (RemoveString.Contains("&#8212;") || RemoveString.Contains("&#8211;"))
            {
                string[] Split = RemoveString.Split(new string[] { "&#8212;", "&#8211;" }, StringSplitOptions.RemoveEmptyEntries);
                RemoveString = "";
                for (int i = 0; i < Split.Length; i++)
                {
                    RemoveString += Split[i];
                    if (i != Split.Length - 1)
                    {
                        RemoveString += "-";
                    }
                }
            }

            // ...
            while (RemoveString.Contains("&#8230;"))
            {
                string[] Split = RemoveString.Split(new string[] { "&#8230;" }, StringSplitOptions.RemoveEmptyEntries);
                RemoveString = "";
                for (int i = 0; i < Split.Length; i++)
                {
                    RemoveString += Split[i];
                    if (i != Split.Length - 1)
                    {
                        RemoveString += "...";
                    }
                }
            }
            return RemoveString;
        }

    }
}
