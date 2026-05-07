# 1. Initialize headers
$pumlHeader = @"
@startuml
skinparam classAttributeIconSize 10
set namespaceSeparator none
left to right direction
"@
$pumlHeader | Out-File -Encoding utf8 full_project.puml

$allTypes = @() 
$primitives = @("int", "float", "double", "bool", "string", "long", "void", "Vector2", "Vector3", "Vector2Int", "Vector3Int", "Quaternion", "Color", "Action", "IEnumerator", "Material", "Bounds", "Mesh", "Rigidbody", "GameObject", "Transform", "Camera", "Text", "UnityEvent", "RaycastHit2D", "LayerMask", "AnimationCurve", "InputActionAsset", "InputActionMap", "InputAction", "SpriteRenderer", "Sprite")

$scripts = Get-ChildItem -Path "..\Assets\Scripts" -Filter *.cs -Recurse

# Pre-scan for types to handle relationships later
foreach ($file in $scripts) {
    $content = Get-Content $file.FullName -Raw
    if ($content -match '(class|interface|struct)\s+(?<name>[\w<>]+)') { 
        $allTypes += $Matches['name'] 
    }
}

$relationships = @()

foreach ($file in $scripts) {
    $fileContent = Get-Content $file.FullName -Raw
    
    # regex to find class/interface and capture the rest of the file
    if ($fileContent -match '(?s)(?<kind>class|interface|struct)\s+(?<name>[\w<>]+)(?:\s*:\s*(?<parents>[^{]+))?\s*\{(?<body>.*)') {
        $typeName = $Matches['name']
        $typeKind = $Matches['kind']
        $parentRaw = $Matches['parents']

        # Fix generic names for PlantUML syntax (wrap in quotes if it has < >)
        $pumlTypeName = if ($typeName -match '<') { "`"$typeName`"" } else { $typeName }

        "$typeKind $pumlTypeName {" | Out-File -Append -Encoding utf8 full_project.puml
        
        $lines = $fileContent -split '[\r\n]+'
        $fields = @()
        $methods = @()
        
        $insideClassScope = $false
        $braceLevel = 0
        $pendingSerialized = $false

        foreach ($line in $lines) {
            $trimmed = $line.Trim()
            if ([string]::IsNullOrWhiteSpace($trimmed)) { continue }

            # TRACKING CLASS ENTRY: Look for the specific class definition line
            if (-not $insideClassScope) {
                if ($trimmed -match "(class|interface|struct)\s+$([regex]::Escape($typeName))") {
                    $insideClassScope = $true
                }
                continue
            }

            # BRACE BALANCING
            $lineBraces = ($trimmed.ToCharArray() | Where-Object { $_ -eq '{' }).Count - ($trimmed.ToCharArray() | Where-Object { $_ -eq '}' }).Count
            
            # If we see a { and we weren't "inside" yet, that's our class start
            if ($trimmed -match '\{' -and $braceLevel -eq 0) {
                $braceLevel += $lineBraces
                continue
            }

            # Update brace level for subsequent lines
            $braceLevel += $lineBraces

            # Only process members if we are EXACTLY at the class body level (Level 1)
            if ($braceLevel -eq 1) {
                # Catch lone attributes
                if ($trimmed -match '^\[SerializeField\]') { 
                    $pendingSerialized = $true 
                    continue 
                }

                $cleanLine = ($trimmed -replace '\[.*?\]', '').Trim()
                if ($cleanLine -match '^[{}]$') { continue }
                if ($cleanLine -match '^using\s+|^namespace\s+') { continue }

                # Visibility
                $vis = "-" 
                if ($cleanLine -match '^public\s+') { $vis = "+" }
                elseif ($cleanLine -match '^protected\s+') { $vis = "#" }
                elseif ($cleanLine -match '^internal\s+') { $vis = "~" }
                elseif ($typeKind -eq "interface") { $vis = "+" }

                # REGEX A: Methods
                if ($cleanLine -match '(?<mods>.*?\s+)?(?<ret>[\w<>\[\]\?\.]+)\s+(?<name>\w+)\s*\(') {
                    $mName = $Matches['name']
                    if ($mName -notmatch 'if|for|while|switch|lock|using') {
                        $methods += "$vis$mName() : $($Matches['ret'])"
                        $pendingSerialized = $false
                    }
                }
                # REGEX B: Fields and Properties
                elseif ($cleanLine -match '(?<mods>.*?\s+)?(?<type>[\w<>\[\]\?\.]+)\s+(?<name>\w+)\s*([;={]|\{\s*get)') {
                    $fName = $Matches['name']
                    $fType = $Matches['type']
                    if ($fName -notmatch 'return|get|set|yield|class') {
                        $sPrefix = if ($pendingSerialized -or $trimmed -match '\[SerializeField\]') { "[S] " } else { "" }
                        $fields += "$vis$sPrefix$fName : $fType"

                        # Build Relationship
                        $cleanType = ($fType -replace '<.*>', '' -replace '\[\]', '').Trim()
                        if ($allTypes -contains $cleanType -and $cleanType -ne $typeName -and $primitives -notcontains $cleanType) {
                            $targetName = if ($cleanType -match '<') { "`"$cleanType`"" } else { $cleanType }
                            $relationships += "$pumlTypeName *-- $targetName"
                        }
                        $pendingSerialized = $false
                    }
                }
            }

            # Exit class if we hit the closing brace
            if ($insideClassScope -and $braceLevel -le 0 -and $trimmed -match '\}') {
                $insideClassScope = $false
                break
            }
        }

        foreach ($f in $fields) { $f | Out-File -Append -Encoding utf8 full_project.puml }
        if ($fields.Count -gt 0 -and $methods.Count -gt 0) { "--" | Out-File -Append -Encoding utf8 full_project.puml }
        foreach ($m in $methods) { $m | Out-File -Append -Encoding utf8 full_project.puml }
        "}" | Out-File -Append -Encoding utf8 full_project.puml

        # Handle Parents / Inheritance
        if ($parentRaw) {
            foreach ($p in ($parentRaw -split ',')) {
                $pName = ($p.Trim() -split '\s+')[0]
                if ($pName -ne "MonoBehaviour" -and ($allTypes -contains $pName -or $pName -eq "IPoolable")) {
                    $pPuml = if ($pName -match '<') { "`"$pName`"" } else { $pName }
                    "$pPuml <|-- $pumlTypeName" | Out-File -Append -Encoding utf8 full_project.puml
                }
            }
        }
    }
}

$relationships | Select-Object -Unique | ForEach-Object { if ($_) { $_ | Out-File -Append -Encoding utf8 full_project.puml } }
"@enduml" | Out-File -Append -Encoding utf8 full_project.puml