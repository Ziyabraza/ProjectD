Make Tests folder located in same directory next to the ProjectD folder, to prevent debugger conflict and cut and paste XunitTest in there.
XunitTest.csproj should also not need aditional refrences or configs this way. 

like this:

├ ProjectD
└ Tests ┬ XunitTests (cut pasted)
        └ Tests.sln (copy pasted) 

still recomended to use:

dotnet clean
and
dotnet restore

for missing pakkages.