using Godot;
using System;

public partial class Globals
{
    internal static Godot.Collections.Dictionary<string, Godot.Collections.Array<string>> roomData =
        new Godot.Collections.Dictionary<string, Godot.Collections.Array<string>> {
            { "LczCommon1", new Godot.Collections.Array<string>{ "lc_room_1_endroom" } },
            { "LczCommon2", new Godot.Collections.Array<string>{ "lc_room_2", "lc_room_2_vent"} },
            { "LczCommon2C", new Godot.Collections.Array<string>{ "lc_room_2c" } },
            { "LczCommon3", new Godot.Collections.Array<string>{ "lc_room_3" } },
            { "LczCommon4", new Godot.Collections.Array<string>{ "lc_room_4" } },
            { "LczSingle1", new Godot.Collections.Array<string>{ "lc_cont_1_079", "lc_room_1_archive" } },
            { "LczSingle2", new Godot.Collections.Array<string>{ "lc_cont_2_012", "lc_cont_2_650", "lc_room_2_hall", "lc_room_2_sl" } },
            { "LczSingle2C", new Godot.Collections.Array<string>{ } },
            { "LczSingle3", new Godot.Collections.Array<string>{ } },
            { "LczSingle4", new Godot.Collections.Array<string>{ } },
        };
}
