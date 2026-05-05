namespace IntegrationTests;

public sealed record TestSeedData(TestSeedPropertyData[] Properties);

public sealed record TestSeedPropertyData(Guid Id, Guid OwnerId, Guid[] DocumentIds);
