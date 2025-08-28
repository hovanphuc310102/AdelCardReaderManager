# ADEL SDK Setup Guide

## Required Files from ADEL SDK

To make your ADEL Card Reader Manager application work, you need to place the ADEL SDK files in the correct location.

### Step 1: Create SDK Directory Structure

Create one of these directory structures in your project:

**Option A: SDK Folder (Recommended)**
```
AdelCardReaderManager/
??? SDK/
?   ??? MAINDLL.DLL
?   ??? [other SDK DLLs if any]
??? Form1.cs
??? Program.cs
??? AdelCardReaderManager.csproj
```

**Option B: Root Directory**
```
AdelCardReaderManager/
??? MAINDLL.DLL
??? Form1.cs
??? Program.cs
??? AdelCardReaderManager.csproj
```

### Step 2: Copy SDK Files

1. **Locate your ADEL SDK files** (usually in a folder like `ADEL SDK` or `Card Reader SDK`)
2. **Copy the following files** from your SDK to the chosen location:
   - `MAINDLL.DLL` (required - main SDK library)
   - Any other `.dll` files that came with the SDK
   - Documentation files (optional, for reference)

### Step 3: Verify File Placement

After copying files, your project structure should look like:

```
AdelCardReaderManager/
??? SDK/                          ? SDK files here (Option A)
?   ??? MAINDLL.DLL               ? Main SDK library
??? bin/Debug/net9.0-windows/     ? Build output (automatic)
??? Form1.cs
??? Program.cs
??? AdelCardReaderManager.csproj
```

### Step 4: Build and Test

1. **Build your project** (Ctrl+Shift+B in Visual Studio)
2. **Check the output directory** (`bin/Debug/net9.0-windows/`)
3. **Verify MAINDLL.DLL is copied** to the output directory
4. **Run your application**
5. **Check the log** - should show "? MAINDLL.DLL found in application directory"

### Step 5: Troubleshooting

If you see "? WARNING: MAINDLL.DLL not found!" in your application:

1. **Check file location**: Ensure MAINDLL.DLL is in the SDK folder or root directory
2. **Check file name**: Must be exactly `MAINDLL.DLL` (case-sensitive)
3. **Rebuild project**: Clean and rebuild to force file copying
4. **Manual copy**: Copy MAINDLL.DLL directly to `bin/Debug/net9.0-windows/`

### Architecture Compatibility

Make sure your SDK DLL matches your application architecture:
- **32-bit SDK** ? requires 32-bit application build
- **64-bit SDK** ? requires 64-bit application build

To check/change your application architecture:
1. Right-click project ? Properties
2. Go to Build ? Platform target
3. Choose `x86` (32-bit), `x64` (64-bit), or `Any CPU`

### Common SDK Files

Typical ADEL SDK includes:
- `MAINDLL.DLL` - Main SDK library (required)
- `ADEL_SDK.pdf` - Documentation (optional)
- Sample code files (optional)
- Additional DLLs for specific card reader models (if any)

## Next Steps

After setting up the SDK:
1. Run your application
2. Click "Check Drivers" to verify USB drivers
3. Click "Diagnostics" for detailed system information
4. Try initializing your card reader

## Need Help?

If you're still having issues:
1. Check the application log output
2. Verify your card reader is connected
3. Ensure USB-to-Serial drivers are installed
4. Try running as Administrator