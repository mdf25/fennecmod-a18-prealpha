using System.Linq;
using Mono.Cecil;
using SDX.Compiler;

/**
 * This patchscript is required when adding new tile entities. It adds a new enum TileEntityType to the existing list of types.
 * Credit to SphereII and Xyth for the script and code examples.
 * 
 * Note: In order to make this work fully, the Harmony patch needs to be installed too in order to allow TileENtity.Instance() to work.
 */

public class EnumPatcher : IPatcherMod
{
    public bool Patch(ModuleDefinition module)
    {
        AddEnumOption(module, "DisplayInfoEntry", "DisplayTypes", "Text", 10);
		return true;
    }

    private void AddEnumOption(ModuleDefinition gameModule, string enumName, string enumFieldName, byte enumValue)
    {
        var enumType = gameModule.Types.First(d => d.Name == enumName);
        FieldDefinition literal = new FieldDefinition(enumFieldName, FieldAttributes.Public | FieldAttributes.Static | FieldAttributes.Literal | FieldAttributes.HasDefault, enumType);
        enumType.Fields.Add(literal);
        literal.Constant = enumValue;
    }
	
	private void AddEnumOption(ModuleDefinition gameModule, string className, string enumName, string enumFieldName, byte enumValue)
    {
        var enumType = gameModule.Types.First(d=> d.Name == className).NestedTypes.First(d => d.Name == enumName);
        FieldDefinition literal = new FieldDefinition(enumFieldName, FieldAttributes.Public | FieldAttributes.Static | FieldAttributes.Literal | FieldAttributes.HasDefault, enumType);
        enumType.Fields.Add(literal);
        literal.Constant = enumValue;
    }
	

    public bool Link(ModuleDefinition gameModule, ModuleDefinition modModule)
    {
        return true;
    }

}