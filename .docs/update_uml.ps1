param (
    [string]$OutputName = "full_project.puml",
    [switch]$HidePrivate
)

# 1. Initialize headers
$pumlHeader = @"
@startuml
skinparam classAttributeIconSize 10
set namespaceSeparator none
left to right direction
"@
$pumlHeader | Out-File -Encoding utf8 $OutputName

$allTypes = @() 
$primitives = @("int", "float", "double", "bool", "string", "long", "void", "Vector2", "Vector3", "Vector2Int", "Vector3Int", "Quaternion", "Color", "Action", "IEnumerator", "Material", "Bounds", "Mesh", "Rigidbody", "GameObject", "Transform", "Camera", "Text", "UnityEvent", "RaycastHit2D", "LayerMask", "AnimationCurve", "InputActionAsset", "InputActionMap", "InputAction", "SpriteRenderer", "Sprite")

$scripts = Get-ChildItem -Path "..\Assets\Scripts" -Filter *.cs -Recurse

# Pre-scan for all types in the project
foreach ($file in $scripts) {
    $content = Get-Content $file.FullName -Raw
    $matches = [regex]::Matches($content, '(?<kind>class|interface|struct)\s+(?<name>[\w<>]+)')
    foreach ($m in $matches) { $allTypes += $m.Groups['name'].Value }
}

$relationships = @()

foreach ($file in $scripts) {
    $fileContent = Get-Content $file.FullName -Raw
    
    # Identify every type and its starting position in the file
    $typeDefinitions = [regex]::Matches($fileContent, '(?s)(?<kind>class|interface|struct)\s+(?<name>[\w<>]+)(?:\s*:\s*(?<parents>[^{]+))?\s*\{')

    foreach ($typeDef in $typeDefinitions) {
        $typeName = $typeDef.Groups['name'].Value
        $typeKind = $typeDef.Groups['kind'].Value
        $parentRaw = $typeDef.Groups['parents'].Value
        $startIndex = $typeDef.Index # The exact character where this class starts

        $pumlTypeName = if ($typeName -match '<') { "`"$typeName`"" } else { $typeName }
        "$typeKind $pumlTypeName {" | Out-File -Append -Encoding utf8 $OutputName
        
        # Only process content from the start of this class onwards
        $remainingContent = $fileContent.Substring($startIndex)
        $lines = $remainingContent -split '[\r\n]+'
        
        $fields = @()
        $methods = @()
        $insideClassScope = $false
        $braceLevel = 0
        $pendingSerialized = $false

        foreach ($line in $lines) {
            $trimmed = $line.Trim()
            if ([string]::IsNullOrWhiteSpace($trimmed)) { continue }

            # Locate the start of THIS specific class using word boundaries
            if (-not $insideClassScope) {
                if ($trimmed -match "\b$typeKind\b\s+\b$([regex]::Escape($typeName))\b") {
                    $insideClassScope = $true
                }
                continue
            }

            $lineBraces = ($trimmed.ToCharArray() | Where-Object { $_ -eq '{' }).Count - ($trimmed.ToCharArray() | Where-Object { $_ -eq '}' }).Count
            
            # Identify the opening brace of the type
            if ($trimmed -match '\{' -and $braceLevel -eq 0) { 
                $braceLevel += $lineBraces
                continue 
            }
            $braceLevel += $lineBraces

            # Only process members belonging to this specific type level
            if ($braceLevel -eq 1) {
                if ($trimmed -match '^\[SerializeField\]') { $pendingSerialized = $true; continue }

                $cleanLine = ($trimmed -replace '\[.*?\]', '').Trim()
                if ($cleanLine -match '^[{}]$') { continue }

                $isExplicitPublic = $cleanLine -match '^public\s+'
                $isProtected = $cleanLine -match '^protected\s+'
                $isInternal = $cleanLine -match '^internal\s+'
                $isSerialized = ($pendingSerialized -or $trimmed -match '\[SerializeField\]')
                
                if ($isExplicitPublic -or ($typeKind -eq "interface")) { $vis = "+" } 
                elseif ($isProtected) { $vis = "#" } 
                elseif ($isInternal) { $vis = "~" } 
                else { $vis = "-" }

                $isPrivate = ($vis -eq "-")
                $shouldSkip = $HidePrivate -and $isPrivate -and (-not $isSerialized)

                # Methods
                if ($cleanLine -match '(?<mods>.*?\s+)?(?<ret>[\w<>\[\]\?\.]+)\s+(?<name>\w+)\s*\(') {
                    $mName = $Matches['name']
                    if ($mName -notmatch 'if|for|while|switch|lock|using') {
                        if (-not $shouldSkip) {
                            $methods += "$vis$mName() : $($Matches['ret'])"
                        }
                        $pendingSerialized = $false
                    }
                }
                # Fields / Properties
                elseif ($cleanLine -match '(?<mods>.*?\s+)?(?<type>[\w<>\[\]\?\.]+)\s+(?<name>\w+)\s*([;={]|\{\s*get)') {
                    $fName = $Matches['name']
                    $fType = $Matches['type']
                    if ($fName -notmatch 'return|get|set|yield|class') {
                        if (-not $shouldSkip) {
                            $sPrefix = if ($isSerialized) { "[S] " } else { "" }
                            $fields += "$vis$sPrefix$fName : $fType"
                        }

                        $cleanType = ($fType -replace '<.*>', '' -replace '\[\]', '').Trim()
                        if ($allTypes -contains $cleanType -and $cleanType -ne $typeName -and $primitives -notcontains $cleanType) {
                            $targetName = if ($cleanType -match '<') { "`"$cleanType`"" } else { $cleanType }
                            
                            if ($fType -match 'List<' -or $fType -match '\[\]') {
                                $relationships += "$pumlTypeName --o $targetName"
                            } elseif ($isExplicitPublic -or $fName -match 'Ref|Manager|Context|Provider') {
                                $relationships += "$pumlTypeName --> $targetName"
                            } else {
                                $relationships += "$pumlTypeName --* $targetName"
                            }
                        }
                        $pendingSerialized = $false
                    }
                }
            }

            # Exit once we close the class brace
            if ($insideClassScope -and $braceLevel -le 0 -and $trimmed -match '\}') {
                break 
            }
        }

        foreach ($f in $fields) { $f | Out-File -Append -Encoding utf8 $OutputName }
        if ($fields.Count -gt 0 -and $methods.Count -gt 0) { "--" | Out-File -Append -Encoding utf8 $OutputName }
        foreach ($m in $methods) { $m | Out-File -Append -Encoding utf8 $OutputName }
        "}" | Out-File -Append -Encoding utf8 $OutputName

        # Inheritance / Nested Relationship
        if ($parentRaw) {
            foreach ($p in ($parentRaw -split ',')) {
                $pName = ($p.Trim() -split '\s+')[0]
                if ($pName -ne "MonoBehaviour" -and ($allTypes -contains $pName -or $pName -eq "IPoolable")) {
                    $pPuml = if ($pName -match '<') { "`"$pName`"" } else { $pName }
                    "$pPuml <|-- $pumlTypeName" | Out-File -Append -Encoding utf8 $OutputName
                }
            }
        }
    }
}

$relationships | Select-Object -Unique | ForEach-Object { if ($_) { $_ | Out-File -Append -Encoding utf8 $OutputName } }
"@enduml" | Out-File -Append -Encoding utf8 $OutputName