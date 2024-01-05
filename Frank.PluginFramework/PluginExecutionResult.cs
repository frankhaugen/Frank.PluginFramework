namespace Frank.PluginFramework;

public readonly record struct PluginExecutionResult(bool Success, Exception? Exception, TimeSpan ExecutionTime);