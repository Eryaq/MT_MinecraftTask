# MT_MinecraftTask

# GenerateChunk
- tato metoda by normálně obsahovala 3 cykly, to je to v pořádku, protože složitost by byla: SizeX * SizeZ * SizeY. U Chunku 16 x 16 x 64 je to 16 384 bloků, což je málo.
- Rozhodl jsem se to ale i tak udělat lépe: Nemusím procházet celý y až do SizeY, když vím, že nad height je vzduch.

# BuildMesh
- tato metoda obsahuje dokonce až 4 cykly, taky to je v pořádku. Mesh builder musí projít a zjistit, které stěny jsou viditelné.
- Složitost je zhruba počet bloků * 6 sousedů. Pro 16 x 16 x 64 to je 16 384 * 6 = 98 304 kontrol. To je pořád v pohodě, pokud se to neprovádí každý frame.

Píšu to, protože se to může zdát neoptimalizovaný. U Voxelů se tomuto zas tak moc vyhnout nedá, takže optimalizaci řeším hlavně tím, že nedělám RebuildMesh každý update, ale jen po změně bloku / při vygenerování chunku.
Navíc to řeším cestou meshe a ne GameObjectu, což už je věc sama o sobě lepší.