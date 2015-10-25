// ReplaceUpdate.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include <windows.h>
#include <Tlhelp32.h>
#include <winbase.h>
#include <string.h>
#include <string>
using namespace std;

void killProc(const char *procName)
{
	HANDLE hSnapShot = CreateToolhelp32Snapshot(TH32CS_SNAPALL, NULL);
	PROCESSENTRY32 pEntry;
	pEntry.dwSize = sizeof(pEntry);
	BOOL hRes = Process32First(hSnapShot, &pEntry);
	while (hRes)
	{
		if (strcmp(pEntry.szExeFile, procName) == 0)
		{
			HANDLE hProcess = OpenProcess(PROCESS_TERMINATE, 0,
				static_cast<DWORD>(pEntry.th32ProcessID));
			if (hProcess != nullptr)
			{
				TerminateProcess(hProcess, 9);
				CloseHandle(hProcess);
			}
		}
		hRes = Process32Next(hSnapShot, &pEntry);
	}
	CloseHandle(hSnapShot);
}

int main()
{
	killProc("SmartBotUI.exe");
	TCHAR NPath[MAX_PATH];
	GetCurrentDirectory(MAX_PATH, NPath);

	string donwloadedFile = string(NPath) + string("\\HearthstoneMulliganNew.dll");
	string oldFile = string(NPath) + string("\\HearthstoneMulligan.dll");
	printf_s(donwloadedFile.c_str());
	printf_s("\n");

	Sleep(3000);
	if (remove(oldFile.c_str()) != 0)
		perror("Error deleting file");
	else
		puts("File successfully deleted");

	rename(donwloadedFile.c_str(), oldFile.c_str());
    return 0;
}

