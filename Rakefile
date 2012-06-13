include Rake::DSL

require 'albacore'

ARTIFACTS_PATH = './artifacts'

task :default => [:compile]

desc "Removes previous build artifacts"
task :clean_artifacts do
  rm_rf "artifacts"
  mkdir "artifacts"
end

desc "Compiles the solution"
msbuild :compile  do |msb|
  msb.properties = {:configuration => "Release", :platform => "Any CPU"}
  msb.targets = [:Clean, :Build]
  msb.solution = "src/Elmah-MongoDB.sln"
  msb.command = "C:/WINDOWS/Microsoft.NET/Framework64/v4.0.30319/MSBuild.exe"
  puts msb.command
end

desc "Creates the TrueFit.Elmah.MongoDB NuGet package"
task :create_nuget_package => [:clean_artifacts, :compile] do
  sh "nuget pack nuget/Package.nuspec -OutputDirectory artifacts"
end

desc "Creates and pushes the NuGet package"
task :push_nuget_package => :create_nuget_package do
  sh "nuget push #{FileList["artifacts/*.nupkg"].gsub(/\//, "\\")} nugetthetruefit -s http://nuget.truefitsolutions.com"
end
