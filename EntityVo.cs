using System;

public class EntityVo
{
	public int uid;

	public string npc_id;

	public EntityType entity_type;

	public int level;

	public int star;

	public int quality;

	public float hp;

	public float mp;

	public int ai_type;

	public int pos;

	public int skin;

	public string atk_priority = string.Empty;

	public string effectId = "Default";

	public EntityVo()
	{
	}

	public EntityVo(EntityType entity_type, string npc_id, int level, int star, int quality, float hp = 0f, float mp = 0f, int ai_type = 0, int skin = 0)
	{
		this.entity_type = entity_type;
		this.npc_id = npc_id;
		this.level = level;
		this.star = star;
		this.quality = quality;
		this.hp = hp;
		this.mp = mp;
		this.ai_type = ai_type;
		this.skin = skin;
	}

	public EntityVo(EntityType entity_type, string npc_id, int pos, int ai_type = 0, string priority = "", string effectId = "Default", int skin = 0)
	{
		this.entity_type = entity_type;
		this.npc_id = npc_id;
		this.pos = pos;
		this.ai_type = ai_type;
		this.atk_priority = priority;
		this.effectId = effectId;
		this.skin = skin;
	}

	public void WriteBuffer(SByteBuffer outBuffer)
	{
		outBuffer.WriteInt(this.uid);
		outBuffer.WriteString(this.npc_id);
		outBuffer.WriteByte((byte)this.entity_type);
		outBuffer.WriteInt(this.level);
		outBuffer.WriteInt(this.star);
		outBuffer.WriteInt(this.quality);
		outBuffer.WriteFloat(this.hp);
		outBuffer.WriteFloat(this.mp);
		outBuffer.WriteInt(this.ai_type);
		outBuffer.WriteInt(this.pos);
	}

	public void ReadFromBuffer(SByteBuffer outBuffer)
	{
		this.uid = outBuffer.ReadInt();
		this.npc_id = outBuffer.ReadString();
		this.entity_type = (EntityType)outBuffer.ReadByte();
		this.level = outBuffer.ReadInt();
		this.star = outBuffer.ReadInt();
		this.quality = outBuffer.ReadInt();
		this.hp = outBuffer.ReadFloat();
		this.mp = outBuffer.ReadFloat();
		this.ai_type = outBuffer.ReadInt();
		this.pos = outBuffer.ReadInt();
	}
}
