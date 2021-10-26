using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace endpoints
{
    public class Startup
    {

        public string Pluralization(int number, string[] words)
        {
            if (number % 10 == 1 && number % 100 != 11)
            {
                return ($"{number}  {words[0]}");
            }
            else if (2 <= number % 100 && number % 100 <= 4)
            {
                return ($"{number}  {words[1]}");
            }
            else
                return ($"{number}  {words[2]}");
        }

        public Dictionary<string, int> numberWordsInText(string text)
        {
            List<string> words = new List<string>(text.ToLower().Split(' '));
            Dictionary<string, int> wordsNumber = new Dictionary<string, int>();
            foreach (string word in words)
            {
                if (!wordsNumber.ContainsKey(word))
                    wordsNumber.Add(word, 1);
                else
                    wordsNumber[word] = wordsNumber[word] + 1;
            }
            return wordsNumber;
        }
        public string uniqueWords(Dictionary<string, int> wordsFrequency)
        {
            string words = "";
            foreach (var word in wordsFrequency)
            {
                if(word.Value == 1)
                    words += $"{word.Key} ";
            }
            return words;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });
                endpoints.MapGet("/headers", async context =>
                {
                    var headers = context.Request.Headers;
                    string result = "";
                    foreach (var header in headers)
                    {
                        result += header.Key + ":" + header.Value + "\n";
                    }
                    await context.Response.WriteAsync(result);
                });

                endpoints.MapGet("/plural", async context =>
                {
                    int number = Convert.ToInt32(context.Request.Query["number"]);
                    string[] words = context.Request.Query["forms"].ToString().Split(',');

                    string result = Pluralization(number, words);
                    await context.Response.WriteAsync(result);

                });
                endpoints.MapPost("/frequency", async context =>
                {
                    string inpText = await new StreamReader(context.Request.Body).ReadToEndAsync();
                    Dictionary<string, int> wordsFrequency = new Dictionary<string, int>();
                    wordsFrequency = numberWordsInText(inpText);
                    string mostUniqueWord = uniqueWords(wordsFrequency);
                    string resultFrequency = "";
                    foreach (var word in wordsFrequency)
                    {
                        resultFrequency += word.Key + ":" + word.Value + "\n";
                    }

                    context.Response.ContentType = "application/json";
                    // context.Response.Headers.Add("Number of unique words", );
                    context.Response.Headers.Add("The most frequent word:", mostUniqueWord);

                    await context.Response.WriteAsJsonAsync(resultFrequency);
                });
            });
        }
    }
}
