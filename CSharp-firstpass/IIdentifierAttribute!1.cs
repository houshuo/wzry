public interface IIdentifierAttribute<TIdentifier>
{
    TIdentifier[] AdditionalIdList { get; }

    TIdentifier ID { get; }
}

