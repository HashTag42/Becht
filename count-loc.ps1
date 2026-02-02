<#
.SYNOPSIS
    Counts lines of code in the SauceDemo solution.

.DESCRIPTION
    Analyzes C# source files and provides a breakdown by category.
    Excludes auto-generated files in obj/ and bin/ directories.

.EXAMPLE
    .\count-loc.ps1
    .\count-loc.ps1 > loc.txt
#>

$projectRoot = $PSScriptRoot
$sourceFiles = Get-ChildItem -Path $projectRoot -Recurse -Include "*.cs" |
    Where-Object { $_.FullName -notmatch "\\(obj|bin)\\" }

$results = @()
$totalLines = 0
$totalCode = 0
$totalBlank = 0
$totalComment = 0

foreach ($file in $sourceFiles) {
    $content = Get-Content $file.FullName
    $lines = $content.Count
    $blank = ($content | Where-Object { $_ -match "^\s*$" }).Count
    $comment = ($content | Where-Object { $_ -match "^\s*(//|/\*|\*)" }).Count
    $code = $lines - $blank - $comment

    # Determine category
    $category = switch -Regex ($file.FullName) {
        "\\Pages\\"  { "Page Objects" }
        "\\Tests\\"  { "Tests" }
        "TestBase"   { "Infrastructure" }
        "TestData"   { "Infrastructure" }
        default      { "Other" }
    }

    $results += [PSCustomObject]@{
        File     = $file.Name
        Category = $category
        Lines    = $lines
        Code     = $code
        Blank    = $blank
        Comment  = $comment
    }

    $totalLines += $lines
    $totalCode += $code
    $totalBlank += $blank
    $totalComment += $comment
}

Write-Output ""
Write-Output "=== Lines of Code Summary ==="
Write-Output ""

# Display by category
$categories = $results | Group-Object Category | Sort-Object Name
foreach ($cat in $categories) {
    $catCode = ($cat.Group | Measure-Object -Property Code -Sum).Sum
    Write-Output "$($cat.Name)"
    foreach ($file in $cat.Group | Sort-Object File) {
        Write-Output "  $($file.File.PadRight(30)) $($file.Code.ToString().PadLeft(5)) lines"
    }
    Write-Output "  $("-" * 40)"
    Write-Output "  Subtotal:                     $($catCode.ToString().PadLeft(5)) lines"
    Write-Output ""
}

# Summary
Write-Output "=== Total ==="
Write-Output "  Files:             $($sourceFiles.Count)"
Write-Output "  Code Lines:        $totalCode"
Write-Output "  Blank Lines:       $totalBlank"
Write-Output "  Comment Lines:     $totalComment"
Write-Output "  Total Lines:       $totalLines"
Write-Output ""
