# Contributing to GPU Fan Controller

First off, thank you for considering contributing to GPU Fan Controller! 🎉

This document provides guidelines for contributing to the project. Following these guidelines helps maintain code quality and makes the contribution process smooth for everyone.

---

## 📋 Table of Contents

- [Code of Conduct](#code-of-conduct)
- [How Can I Contribute?](#how-can-i-contribute)
- [Development Setup](#development-setup)
- [Coding Standards](#coding-standards)
- [Pull Request Process](#pull-request-process)
- [Testing Guidelines](#testing-guidelines)

---

## 🤝 Code of Conduct

### Our Standards

- ✅ Be respectful and inclusive
- ✅ Welcome newcomers and help them learn
- ✅ Focus on what's best for the community
- ✅ Show empathy towards other contributors
- ❌ No harassment, trolling, or insulting comments
- ❌ No personal or political attacks

---

## 🎯 How Can I Contribute?

### 🐛 Reporting Bugs

**Before submitting a bug report:**
1. Check the [existing issues](https://github.com/yourusername/gpu-fan-controller/issues)
2. Update to the latest version
3. Test with administrator/sudo privileges

**Bug Report Should Include:**
- Clear, descriptive title
- Steps to reproduce the bug
- Expected vs. actual behavior
- Screenshots (if applicable)
- System information:
  - OS version
  - GPU model and driver version
  - .NET runtime version
  - Application version

**Example:**
```markdown
**Bug**: Fan speed not applying on AMD Radeon RX 6800

**Steps to Reproduce:**
1. Select AMD GPU from dropdown
2. Enable manual control
3. Set fan speed to 70%
4. Click "Apply"

**Expected**: Fan speed changes to 70%
**Actual**: Fan speed stays at default

**System Info:**
- OS: Windows 11 Pro 22H2
- GPU: AMD Radeon RX 6800 XT
- Driver: Adrenalin 23.11.1
- App Version: 2.3.2
```

---

### 💡 Suggesting Features

**Feature requests should include:**
- Clear description of the feature
- Why it would be useful
- Possible implementation approach
- Mockups/examples (if UI-related)

---

### 🔧 Code Contributions

#### Good First Issues

Look for issues labeled:
- `good first issue` - Easy for beginners
- `help wanted` - We need help on these
- `documentation` - Improve docs

#### Areas We Need Help

- 🐧 Linux support improvements
- 🎨 UI/UX enhancements
- 📝 Documentation
- 🧪 Testing on different hardware
- 🌍 Translations/localization
- 🐛 Bug fixes

---

## 🛠️ Development Setup

### Prerequisites

- [.NET 6.0 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
- Windows: [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)
- Linux: VS Code or any editor with C# support
- Git

### Clone and Build

```bash
# 1. Fork the repository on GitHub
# 2. Clone your fork
git clone https://github.com/YOUR_USERNAME/gpu-fan-controller.git
cd gpu-fan-controller

# 3. Add upstream remote
git remote add upstream https://github.com/ORIGINAL_OWNER/gpu-fan-controller.git

# 4. Build the project
dotnet build GPUFanController.csproj -c Debug
dotnet build GPUFanControllerConsole.csproj -c Debug

# 5. Run tests (when available)
dotnet test
```

### Setting Up Analytics (Optional)

If you want to test analytics during development:

```bash
# Set environment variables
export GA_MEASUREMENT_ID="your_test_id"
export GA_API_SECRET="your_test_secret"

# Or use the provided script (Windows)
# Edit set-analytics-env.bat with your test credentials
# DO NOT commit this file!
```

---

## 📏 Coding Standards

### C# Style Guide

We follow [Microsoft's C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions).

**Key Points:**

```csharp
// ✅ Good: PascalCase for public members
public class GPUController
{
    public int FanSpeed { get; set; }
    
    public void SetFanSpeed(int speed)
    {
        // Implementation
    }
}

// ✅ Good: camelCase for private fields with underscore prefix
private readonly MultiGPUController _multiGPUController;
private int _selectedGPUIndex;

// ✅ Good: Descriptive variable names
int targetFanSpeed = CalculateFanSpeed(temperature);

// ❌ Bad: Single letter variables (except in loops)
int s = GetSpeed(t);

// ✅ Good: Comments for complex logic
// Apply hysteresis to prevent rapid fan speed oscillations
// This smooths out temporary temperature spikes
if (Math.Abs(newSpeed - currentSpeed) < hysteresis)
{
    return currentSpeed;
}

// ✅ Good: XML documentation for public APIs
/// <summary>
/// Sets the fan speed for the specified GPU.
/// </summary>
/// <param name="gpuIndex">Zero-based GPU index</param>
/// <param name="speed">Fan speed percentage (0-100)</param>
/// <returns>True if successful, false otherwise</returns>
public bool SetFanSpeed(int gpuIndex, int speed)
```

### File Organization

```
- One class per file
- File name matches class name
- Group related functionality
- Keep files under 1000 lines
```

### Error Handling

```csharp
// ✅ Good: Specific exceptions, user-friendly messages
try
{
    SetFanSpeed(gpuIndex, speed);
}
catch (HardwareAccessException ex)
{
    MessageBox.Show(
        "Failed to control GPU fan. Please ensure:\n" +
        "1. Running as Administrator\n" +
        "2. GPU supports software fan control\n" +
        $"Error: {ex.Message}",
        "Hardware Error",
        MessageBoxButtons.OK,
        MessageBoxIcon.Error
    );
}

// ❌ Bad: Swallowing exceptions silently
try
{
    SetFanSpeed(gpuIndex, speed);
}
catch { }
```

---

## 🔄 Pull Request Process

### Before Submitting

1. ✅ Create a feature branch
   ```bash
   git checkout -b feature/my-awesome-feature
   ```

2. ✅ Make your changes following coding standards

3. ✅ Test thoroughly on your hardware

4. ✅ Update documentation if needed

5. ✅ Commit with clear messages
   ```bash
   git commit -m "Add support for AMD RX 7000 series fan control"
   ```

6. ✅ Push to your fork
   ```bash
   git push origin feature/my-awesome-feature
   ```

### Pull Request Template

```markdown
## Description
Brief description of what this PR does.

## Type of Change
- [ ] Bug fix
- [ ] New feature
- [ ] Breaking change
- [ ] Documentation update

## Testing
- [ ] Tested on NVIDIA GPU
- [ ] Tested on AMD GPU
- [ ] Tested on Intel GPU
- [ ] Tested on Windows
- [ ] Tested on Linux
- [ ] Tested automatic mode
- [ ] Tested manual mode

## Hardware Tested
- GPU Model: RTX 3080
- Driver Version: 536.23
- OS: Windows 11 22H2

## Screenshots (if applicable)
[Add screenshots here]

## Additional Notes
Any additional information reviewers should know.
```

### Review Process

1. Maintainers will review your PR
2. Address any requested changes
3. Once approved, maintainers will merge
4. Your contribution will be in the next release! 🎉

---

## 🧪 Testing Guidelines

### Manual Testing Checklist

**Before submitting GPU-related changes:**

#### Basic Functionality
- [ ] Application launches without errors
- [ ] GPU detection works
- [ ] Temperature monitoring updates
- [ ] Fan speed displays correctly

#### Manual Control
- [ ] Fan speed slider works
- [ ] Apply button sets fan speed
- [ ] Safety warnings appear for low speeds
- [ ] Reset to auto works

#### Automatic Control
- [ ] All 4 profiles load correctly
- [ ] Fan speed adjusts based on temperature
- [ ] Auto mode can be stopped
- [ ] Temperature thresholds are correct

#### Multi-GPU
- [ ] Multiple GPUs detected
- [ ] Can switch between GPUs
- [ ] Each GPU controlled independently

#### Safety
- [ ] Safety warnings appear appropriately
- [ ] Temperature color coding works
- [ ] Reset to auto on exit works

### Hardware Diversity

**Test on multiple brands if possible:**
- NVIDIA (GeForce, RTX, Quadro)
- AMD (Radeon RX, Vega, RDNA)
- Intel (Arc, Xe)

**If you can't test on all hardware:**
- Clearly state what you tested on
- Other contributors can help test

---

## 📝 Documentation

### Types of Documentation

1. **Code Comments**
   - Explain *why*, not *what*
   - Document complex algorithms
   - Add XML docs for public APIs

2. **README Updates**
   - Update if adding features
   - Add usage examples
   - Update requirements

3. **Wiki/Guides**
   - Troubleshooting guides
   - Hardware compatibility lists
   - Configuration examples

---

## 🏷️ Versioning

We use [Semantic Versioning](https://semver.org/):

- **MAJOR**: Breaking changes
- **MINOR**: New features (backwards compatible)
- **PATCH**: Bug fixes

---

## 📞 Questions?

- 💬 [Open a discussion](https://github.com/yourusername/gpu-fan-controller/discussions)
- 📧 Contact maintainers
- 📖 Check existing docs

---

## 🙏 Thank You!

Every contribution, no matter how small, is appreciated:
- Bug reports help improve stability
- Feature suggestions drive innovation
- Code contributions move the project forward
- Documentation helps new users

**Happy coding! 🚀**
