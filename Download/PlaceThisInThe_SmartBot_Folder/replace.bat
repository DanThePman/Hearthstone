@echo off
taskkill /f /t /im "SmartBotUI.exe"
timeout 5
erase HearthstoneMulligan.dll
rename HearthstoneMulliganNew.dll HearthstoneMulligan.dll
pause
