namespace SchmidiDB.Storage;

public record AttributeType
{
    public record IntValue(int value): AttributeType;

    public record StringValue(String value): AttributeType;
    
    private AttributeType() {}
    
}