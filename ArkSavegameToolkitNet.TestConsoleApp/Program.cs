﻿using ArkSavegameToolkitNet.DataConsumers;
using ArkSavegameToolkitNet.Domain;
using log4net.Repository;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace ArkSavegameToolkitNet.TestConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var savePath = @"D:\asmdata\Servers\Server4\ShooterGame\Saved\\SavedArks\Ragnarok_ArkBot.ark";
            var clusterPath = @"D:\asmdata\clusters\JustForFunClusertID1";
            var domainOnly = true; //true: optimize loading of the domain model, false: load everything and keep references in memory

            // initialize default settings (maps etc.)
            ArkToolkitDomain.Initialize();

            //prepare
            var cd = new ArkClusterData(clusterPath, loadOnlyPropertiesInDomain: domainOnly);
            var gd = new ArkGameData(savePath, cd, loadOnlyPropertiesInDomain: domainOnly);

            var st = Stopwatch.StartNew();
            //extract savegame
            if (gd.Update(CancellationToken.None, null, true)?.Success == true)
            {
                Console.WriteLine($@"Elapsed (gd) {st.ElapsedMilliseconds:N0} ms");
                st = Stopwatch.StartNew();

                //extract cluster data
                var clusterResult = cd.Update(CancellationToken.None);

                Console.WriteLine($@"Elapsed (cd) {st.ElapsedMilliseconds:N0} ms");
                st = Stopwatch.StartNew();

                //assign the new data to the domain model
                gd.ApplyPreviousUpdate(domainOnly);

                Console.WriteLine($@"Elapsed (gd-apply) {st.ElapsedMilliseconds:N0} ms");

                Console.WriteLine("Save data loaded!");
            }
            else
            {
                Console.WriteLine("Failed to load save data!");
            }

            Console.WriteLine("Press any key...");
            Console.ReadKey();
        }
    }
}