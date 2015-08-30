using System;
using System.Collections.Generic;
using System.Text;
using ClearbytesBridge;

namespace Clearbytes.Modules
{
    [ClearbytesModuleAttributes("Test module", "This is a test module, you shouldn't be able to see this in a public release.", false)]
    public class TestModule : ClearbytesModule
    {
        public override void Search()
        {
            this.AddInformation("Some text node", InformationType.Text, "Some magical text data");
            this.AddInformation("Some image node", InformationType.Image, Properties.Resources.icon128_a);
            this.AddInformation("Some binary node", InformationType.Binary, Encoding.ASCII.GetBytes("Some magical binary data"));
            this.AddInformation("Some actual binary node", InformationType.Binary, new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 0x00, 0x00, 0x00, 0x0D, 0x49, 0x48, 0x44, 0x52, 0x00, 0x00, 0x01, 0x4A, 0x00, 0x00, 0x01, 0x74, 0x08, 0x06, 0x00, 0x00, 0x00, 0x45, 0x59, 0x70, 0xD8 });

            this.AddInformation("Some expandable node", InformationType.Title, new TitleInfo("Some title", "Some description"))
                .AddInformation("Some nested text node", InformationType.Text, "Some magical nested data")
                    .AddInformation("Some nested image node", InformationType.Image, Properties.Resources.icon128_a)
                        .AddInformation("Some nested binary node", InformationType.Binary, Encoding.ASCII.GetBytes("Some magical binary data"));
        }
    }
}
