using Discord.Commands;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Multivac.Utilities
{
    public static class EvalService
    {
        public static async Task EvaluateAsync(string evalstring, SocketCommandContext context)
        {
            var message = await context.Channel.SendMessageAsync($"processing. . .");

            string[] references =
            {
                "Discord", "Discord.Commands", "Discord.WebSocket",
                "System", "System.Collections.Generic", "System.Linq", "System.Text", "System.Threading.Tasks",
                "SharpLink"
            };

            var scriptOptions = ScriptOptions.Default.AddReferences(references);

            var globals = new Globals
            {
                Context = context
            };


            try
            {
                var result = await CSharpScript.EvaluateAsync($"{string.Join(" ", references.Select(x => $"using {x};"))} {evalstring}", scriptOptions, globals);

                await message.ModifyAsync(x => x.Content = $"results: {result ?? ""}");
            }
            catch (CompilationErrorException e)
            {
                await message.ModifyAsync(x => x.Content = "error:\n" + string.Join(Environment.NewLine, e.Diagnostics));
            }
            finally
            {
                GC.Collect(); //?fixes mem leaks?!?
                GC.WaitForPendingFinalizers();
            }

        }
    }



    public class Globals
    {
        public SocketCommandContext Context;
    }
}
