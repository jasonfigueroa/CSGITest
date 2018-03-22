using Q42.HueApi;
using Q42.HueApi.ColorConverters;
using Q42.HueApi.ColorConverters.HSB;
using Q42.HueApi.Interfaces;
using Q42.HueApi.Models.Groups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CSGITest
{
    class LightInterface
    {
        public static bool alive;

        public static async Task BombActive()
        {
            ILocalHueClient client = new LocalHueClient("192.168.1.104");
            client.Initialize("7R70D9970VOrmdffj3qwNgUBpNwu4d7I12IZKTqj");
            var command = new LightCommand();
            command.TurnOn().SetColor(new RGBColor("FF0000"));
            command.Alert = Alert.Multiple;

            while (alive)
            {
                await client.SendCommandAsync(command);
                Thread.Sleep(7000);
            }
            command.TurnOff();
            command.Alert = Alert.None;
            await client.SendCommandAsync(command);
        }

        public static async Task BombDefused()
        {
            ILocalHueClient client = new LocalHueClient("192.168.1.104");
            client.Initialize("7R70D9970VOrmdffj3qwNgUBpNwu4d7I12IZKTqj");
            var command = new LightCommand();
            command.TurnOn().SetColor(new RGBColor("0000FF"));

            command.Alert = Alert.None;

            await client.SendCommandAsync(command);

            Thread.Sleep(10000);
        }

        public static async Task BombExplode()
        {
            ILocalHueClient client = new LocalHueClient("192.168.1.104");
            client.Initialize("7R70D9970VOrmdffj3qwNgUBpNwu4d7I12IZKTqj");
            var command = new LightCommand();
            command.TurnOn().SetColor(new RGBColor("FFA500"));

            command.Alert = Alert.None;

            await client.SendCommandAsync(command);

            Thread.Sleep(10000);
        }

        public static async Task TurnLightOff()
        {
            ILocalHueClient client = new LocalHueClient("192.168.1.104");
            client.Initialize("7R70D9970VOrmdffj3qwNgUBpNwu4d7I12IZKTqj");
            var command = new LightCommand();
            command.TurnOff();

            await client.SendCommandAsync(command);
        }
    }
}
