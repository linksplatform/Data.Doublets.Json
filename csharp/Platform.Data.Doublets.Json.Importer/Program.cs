using TLinkAddress = System.UInt64;

namespace Platform.Data.Doublets.Json.Importer
{
    class Program
    {
        /// <summary>
        /// <para>
        /// Main the args.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="args">
        /// <para>The args.</para>
        /// <para></para>
        /// </param>
        static void Main(params string[] args)
        {
            new JsonImporterCli<TLinkAddress>().Run(args);
        }
    }
}