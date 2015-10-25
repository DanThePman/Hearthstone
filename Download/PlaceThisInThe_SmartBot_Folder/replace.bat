@echo off
taskkill /f /im "MulliganTester.exe"
timeout 5
erase HearthstoneMulligan.dll
rename HearthstoneMulliganNew.dll HearthstoneMulligan.dll
pause
