@ECHO OFF

REM SET BinariesFolder="%~dp0..\..\Dependencies\InternalRepository\."
REM IF NOT EXIST %BinariesFolder% MD %BinariesFolder%
nuget.exe pack "PatternMatch\PatternMatch.nuspec" -IncludeReferencedProjects -Prop Configuration=Release -Prop Platform=AnyCPU 
REM -OutputDirectory %BinariesFolder%