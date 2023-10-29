using Godot;
using System;

public partial class Globals
{
    //Change these values for HCZ and EZ.
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
            /*{ "EzCommon1", new Godot.Collections.Array<string>{ "lc_room_1_endroom" } },
            { "EzCommon2", new Godot.Collections.Array<string>{ "rz_room_2"} },
            { "EzCommon2C", new Godot.Collections.Array<string>{ "rz_room_2c" } },
            { "EzCommon3", new Godot.Collections.Array<string>{ "rz_room_3" } },
            { "EzCommon4", new Godot.Collections.Array<string>{ "rz_room_4" } },
            { "EzSingle1", new Godot.Collections.Array<string>{ } },
            { "EzSingle2", new Godot.Collections.Array<string>{ "rz_room_2_offices", "rz_room_2_offices_2", "rz_room_2_poffices", "rz_room_2_toilets",
                "rz_room_2_cafeteria", "rz_room_2_servers" } },
            { "EzSingle2C", new Godot.Collections.Array<string>{ } },
            { "EzSingle3", new Godot.Collections.Array<string>{ } },
            { "EzSingle4", new Godot.Collections.Array<string>{ } },
            { "HczCommon1", new Godot.Collections.Array<string>{ "hc_room_1_endroom" } },
            { "HczCommon2", new Godot.Collections.Array<string>{ "hc_room_2"} },
            { "HczCommon2C", new Godot.Collections.Array<string>{ "hc_room_2c" } },
            { "HczCommon3", new Godot.Collections.Array<string>{ "hc_room_3" } },
            { "HczCommon4", new Godot.Collections.Array<string>{ "hc_room_4" } },
            { "HczSingle1", new Godot.Collections.Array<string>{ "hc_cont_1_173", "hc_cont_1_106", "hc_cont_1_049" } },
            { "HczSingle2", new Godot.Collections.Array<string>{ "hc_room_2_nuke", "hc_cont_2_testroom" } },
            { "HczSingle2C", new Godot.Collections.Array<string>{ } },
            { "HczSingle3", new Godot.Collections.Array<string>{ } },
            { "HczSingle4", new Godot.Collections.Array<string>{ } },
            */
        };
}
