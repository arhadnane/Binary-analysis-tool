# 🧪 Unit Tests - Versatile Binary Analyzer

## 📊 Couverture des Tests

### ✅ Tests Implémentés

#### `BinaryParserTests.cs`
- **Conversion binaire → bytes** : Validation des chaînes binaires valides/invalides
- **Conversion ASCII** : Test de décodage en ASCII
- **Conversion Base64** : Vérification de l'encodage Base64
- **Conversion Hex** : Test de la conversion en hexadécimal
- **Gestion d'erreurs** : Validation des exceptions pour entrées invalides

#### `FileAnalyzerTests.cs`
- **Détection PNG** : Signature `89 50 4E 47`
- **Détection JPEG** : Signature `FF D8 FF`
- **Détection PDF** : Signature `25 50 44 46`
- **Détection ZIP** : Signature `50 4B 03 04`
- **Détection GIF** : Signature `47 49 46 38`
- **Cas limites** : Fichiers inconnus, vides, signatures partielles

#### `EntropyTests.cs`
- **Entropie maximale** : Distribution uniforme (≈8.0)
- **Entropie nulle** : Données identiques (=0.0)
- **Entropie binaire** : Deux valeurs équiprobables (=1.0)
- **Cas limites** : Tableaux vides, null, un seul élément
- **Données texte** : Entropie modérée (2-4)

#### `DisassemblerTests.cs`
- **Désassemblage basique** : Affichage des bytes bruts
- **Mode 32/64-bit** : Test des deux architectures
- **Gestion des données vides** : Retour correct
- **Limitation à 16 bytes** : Troncature des gros fichiers

#### `ExtensionsTests.cs`
- **Méthodes d'extension** : ToHex(), ToAscii()
- **Cas limites** : Tableaux vides, bytes non-imprimables
- **Cohérence** : Comparaison avec les méthodes principales

## 🚀 Exécution des Tests

### Commandes principales
```bash
# Tous les tests
dotnet test

# Tests avec détails
dotnet test --verbosity normal

# Tests d'un projet spécifique
dotnet test BinaryAnalyzer.Tests/

# Tests avec couverture de code
dotnet test --collect:"XPlat Code Coverage"
```

### Résultats attendus
- **Nombre total** : ~25 tests
- **Taux de réussite** : 100%
- **Couverture** : >90% des méthodes publiques

## 📈 Métriques de Qualité

### Tests par catégorie
- **Tests unitaires** : 25 tests
- **Tests d'intégration** : 0 (à ajouter)
- **Tests de performance** : 0 (à ajouter)

### Frameworks utilisés
- **xUnit** : Framework de tests principal
- **Microsoft.NET.Test.Sdk** : SDK de tests .NET
- **Coverlet** : Analyse de couverture de code

## 🔧 Tests à Ajouter (Roadmap)

### Tests d'intégration
- **CLI End-to-End** : Test complet via `Program.Main()`
- **Fichiers réels** : Test avec vrais fichiers PNG/PDF/ZIP
- **Performance** : Tests avec gros fichiers (>100MB)

### Tests de régression
- **Formats exotiques** : Signatures de fichiers rares
- **Données corrompues** : Fichiers partiellement corrompus
- **Edge cases** : Fichiers de 0 byte, très gros fichiers

### Tests de sécurité
- **Dépassement de mémoire** : Protection contre les gros fichiers
- **Injection** : Validation des chemins de fichiers
- **DoS** : Résistance aux fichiers malformés

## 📝 Bonnes Pratiques

### Conventions de nommage
- **Méthode** : `MethodName_Scenario_ExpectedResult`
- **Classe** : `ClassNameTests`
- **Données** : Variables explicites (arrange/act/assert)

### Structure AAA
```csharp
[Fact]
public void Method_Scenario_Expected()
{
    // Arrange
    var input = CreateTestData();
    
    // Act
    var result = MethodUnderTest(input);
    
    // Assert
    Assert.Equal(expected, result);
}
```

---
*Tests créés le 4 juillet 2025*
