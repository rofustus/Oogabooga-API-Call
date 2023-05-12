using Newtonsoft.Json;
using System.Text;
using System.Speech.Recognition;
using System.Speech.Synthesis;
class Program
{
    static async Task Main(string[] args)
    {
        SpeechRecognitionEngine recognizer = new SpeechRecognitionEngine();
        SpeechSynthesizer synth = new SpeechSynthesizer();
        // Configure the recognizer to use the default system microphone
        recognizer.SetInputToDefaultAudioDevice();

        // Create a new grammar for the speech recognizer to use
        GrammarBuilder grammarBuilder = new GrammarBuilder();
        grammarBuilder.AppendDictation();

        synth.SelectVoiceByHints(VoiceGender.Female, VoiceAge.Adult);
        synth.Rate = 1;
        string prompt = "";
        while (true)
        {
            Grammar grammar = new Grammar(grammarBuilder);

            // Load the grammar into the speech recognizer
            recognizer.LoadGrammar(grammar);
            RecognitionResult result = recognizer.Recognize();
            prompt = result.Text;
            Console.WriteLine(prompt);
            //Console.WriteLine("Hello, World!");
            //prompt = Console.ReadLine();
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:5000/api/v1/generate");
            var content = new StringContent("{\r\n    \"prompt\": \"" + prompt + "\",\r\n    \"max_new_tokens\": 25,\r\n    \"do_sample\": true,\r\n    \"temperature\": 1.3,\r\n    \"top_p\": 0.1,\r\n    \"typical_p\": 1,\r\n    \"repetition_penalty\": 1.18,\r\n    \"top_k\": 40,\r\n    \"min_length\": 0,\r\n    \"no_repeat_ngram_size\": 0,\r\n    \"num_beams\": 1,\r\n    \"penalty_alpha\": 0,\r\n    \"length_penalty\": 1,\r\n    \"early_stopping\": false,\r\n    \"seed\": -1,\r\n    \"add_bos_token\": true,\r\n    \"truncation_length\": 2048,\r\n    \"ban_eos_token\": false,\r\n    \"skip_special_tokens\": true,\r\n    \"stopping_strings\": []\r\n}", Encoding.UTF8, "application/json");
            request.Content = content;
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            dynamic parsedJson = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync());
            string text = parsedJson.results[0].text;

            Console.WriteLine(text);//await response.Content.ReadAsStringAsync());
            synth.Speak(text);
            

        }
      
    }
}
