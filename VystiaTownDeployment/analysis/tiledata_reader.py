"""
Tiledata reader for classifying land/static tiles by flags and names.
"""

import struct
from dataclasses import dataclass
from enum import IntFlag
from pathlib import Path
from typing import Dict, Optional


class TileFlags(IntFlag):
    None_ = 0x00000000
    Background = 0x00000001
    Weapon = 0x00000002
    Transparent = 0x00000004
    Translucent = 0x00000008
    Wall = 0x00000010
    Damaging = 0x00000020
    Impassable = 0x00000040
    Wet = 0x00000080
    Unknown1 = 0x00000100
    Surface = 0x00000200
    Bridge = 0x00000400
    Generic = 0x00000800
    Window = 0x00001000
    NoShoot = 0x00002000
    ArticleA = 0x00004000
    ArticleAn = 0x00008000
    Internal = 0x00010000
    Foliage = 0x00020000
    PartialHue = 0x00040000
    Unknown2 = 0x00080000
    Map = 0x00100000
    Container = 0x00200000
    Wearable = 0x00400000
    LightSource = 0x00800000
    Animation = 0x01000000
    HoverOver = 0x02000000
    Unknown3 = 0x04000000
    Roof = 0x08000000
    Door = 0x10000000
    StairBack = 0x20000000
    StairRight = 0x40000000
    AlphaBlend = 0x80000000


@dataclass
class TileInfo:
    tile_id: int
    flags: TileFlags
    height: int
    name: str


class TileDataReader:
    LAND_GROUPS = 512
    LAND_ENTRY_SIZE = 26
    LAND_GROUP_SIZE = 4 + (32 * LAND_ENTRY_SIZE)
    STATIC_ENTRY_SIZE = 37
    STATIC_GROUP_SIZE = 4 + (32 * STATIC_ENTRY_SIZE)

    def __init__(self, tiledata_path: str):
        self.tiledata_path = Path(tiledata_path)
        self.file = open(self.tiledata_path, 'rb')
        self._land_cache: Dict[int, TileInfo] = {}
        self._static_cache: Dict[int, TileInfo] = {}
        self._land_section_size = self.LAND_GROUPS * self.LAND_GROUP_SIZE

    def close(self):
        try:
            self.file.close()
        except Exception:
            pass

    def _read_name(self) -> str:
        name_bytes = self.file.read(20)
        return name_bytes.split(b'\0', 1)[0].decode('utf-8', 'ignore').strip()

    def get_land(self, tile_id: int) -> Optional[TileInfo]:
        if tile_id < 0 or tile_id >= 0x4000:
            return None
        if tile_id in self._land_cache:
            return self._land_cache[tile_id]

        group = tile_id // 32
        entry_in_group = tile_id % 32
        group_offset = group * self.LAND_GROUP_SIZE
        entry_offset = group_offset + 4 + entry_in_group * self.LAND_ENTRY_SIZE

        self.file.seek(entry_offset)
        flags = TileFlags(struct.unpack('<I', self.file.read(4))[0])
        self.file.read(2)  # texture id
        name = self._read_name()

        info = TileInfo(tile_id=tile_id, flags=flags, height=0, name=name)
        self._land_cache[tile_id] = info
        return info

    def get_static(self, tile_id: int) -> Optional[TileInfo]:
        if tile_id < 0:
            return None
        if tile_id in self._static_cache:
            return self._static_cache[tile_id]

        group = tile_id // 32
        entry_in_group = tile_id % 32
        static_offset = self._land_section_size + group * self.STATIC_GROUP_SIZE
        entry_offset = static_offset + 4 + entry_in_group * self.STATIC_ENTRY_SIZE

        self.file.seek(entry_offset)
        flags = TileFlags(struct.unpack('<I', self.file.read(4))[0])
        self.file.read(1)  # weight
        self.file.read(1)  # quality
        self.file.read(6)  # unknown
        self.file.read(1)  # unknown1
        self.file.read(1)  # quantity
        self.file.read(2)  # anim id
        self.file.read(1)  # unknown2
        self.file.read(1)  # hue
        self.file.read(2)  # stack offset
        height = struct.unpack('B', self.file.read(1))[0]
        name = self._read_name()

        info = TileInfo(tile_id=tile_id, flags=flags, height=height, name=name)
        self._static_cache[tile_id] = info
        return info
