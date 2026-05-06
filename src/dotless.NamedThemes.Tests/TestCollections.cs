using Xunit;

// Prevents parallel execution across test classes that share NamedThemesConfig.Options
// and MemoryCache.Default (both static).
[assembly: CollectionBehavior(DisableTestParallelization = true)]
