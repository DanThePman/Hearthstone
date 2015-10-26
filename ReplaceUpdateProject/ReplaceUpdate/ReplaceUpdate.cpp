// ReplaceUpdate.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include <windows.h>
#include <Tlhelp32.h>
#include <winbase.h>
#include <string.h>
#include <string>
#include <iostream>

#pragma comment(lib, "version.lib")
#include <sstream>
#include <fstream>

using namespace std;

VOID GetFileVersion(const TCHAR *pszFilePath, string& version)
{
	DWORD dwSize = 0;
	BYTE* pbVersionInfo = nullptr;
	VS_FIXEDFILEINFO* pFileInfo = nullptr;
	UINT puLenFileInfo = 0;

	// get the version info for the file requested
	dwSize = GetFileVersionInfoSize(pszFilePath, nullptr);
	if (dwSize == 0)
	{
		printf("Error in GetFileVersionInfoSize: %d\n", GetLastError());
		return;
	}

	pbVersionInfo = new BYTE[dwSize];

	if (!GetFileVersionInfo(pszFilePath, 0, dwSize, pbVersionInfo))
	{
		printf("Error in GetFileVersionInfo: %d\n", GetLastError());
		delete[] pbVersionInfo;
		return;
	}

	if (!VerQueryValue(pbVersionInfo, TEXT("\\"), reinterpret_cast<LPVOID*>(&pFileInfo), &puLenFileInfo))
	{
		printf("Error in VerQueryValue: %d\n", GetLastError());
		delete[] pbVersionInfo;
		return;
	}


	std::ostringstream s1, s2;
	DWORD a = (pFileInfo->dwProductVersionMS >> 16) & 0xff;
	DWORD b = (pFileInfo->dwProductVersionMS >> 0) & 0xff;
	s1 << a;
	s2 << b;

	version = s1.str() + string(".") + s2.str();
}

BOOL killProc(const char* procName, HANDLE &proc_handle)
{
	HANDLE hSnapShot = CreateToolhelp32Snapshot(TH32CS_SNAPALL, NULL);
	PROCESSENTRY32 pEntry;
	pEntry.dwSize = sizeof(pEntry);
	BOOL hRes = Process32First(hSnapShot, &pEntry);
	while (hRes)
	{
		if (strcmp(pEntry.szExeFile, procName) == NULL)
		{
			HANDLE hProcess = OpenProcess(PROCESS_TERMINATE, 0, static_cast<DWORD>(pEntry.th32ProcessID));
			if (hProcess != nullptr)
			{
				proc_handle = hProcess;
				TerminateProcess(hProcess, 9);
				CloseHandle(hProcess);
			}
		}
		hRes = Process32Next(hSnapShot, &pEntry);
	}
	CloseHandle(hSnapShot);

	return true;
}

inline bool exists(const std::string& name) {
	ifstream f(name.c_str());
	if (f.good()) {
		f.close();
		return true;
	}
	else {
		f.close();
		return false;
	}
}

/*starts smartbot*/
void startup(const TCHAR* lpApplicationName)
{
	STARTUPINFO si = {};
	si.cb = sizeof si;

	PROCESS_INFORMATION pi = {};
	const TCHAR* target = lpApplicationName;

	if (CreateProcess(target, nullptr, nullptr, nullptr, 0, 0, nullptr, nullptr, &si, &pi))
	{
		WaitForSingleObject(pi.hProcess, 500);


		//SmartBot V31.10 - Game : disconnected - Auth : connected
		string version;
		GetFileVersion(lpApplicationName, version);
		string sb_Caption = string("SmartBot V") + version + string(" - Game : disconnected - Auth : connected\n");

		printf_s(sb_Caption.c_str());

		CloseHandle(pi.hProcess);
		CloseHandle(pi.hThread);
	}
}

DWORD FindProcessId(char* processName)
{
	// strip path

	char* p = strrchr(processName, '\\');
	if (p)
		processName = p + 1;

	PROCESSENTRY32 processInfo;
	processInfo.dwSize = sizeof(processInfo);

	HANDLE processesSnapshot = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, NULL);
	if (processesSnapshot == INVALID_HANDLE_VALUE)
		return 0;

	Process32First(processesSnapshot, &processInfo);
	if (!strcmp(processName, processInfo.szExeFile))
	{
		CloseHandle(processesSnapshot);
		return processInfo.th32ProcessID;
	}

	while (Process32Next(processesSnapshot, &processInfo))
	{
		if (!strcmp(processName, processInfo.szExeFile))
		{
			CloseHandle(processesSnapshot);
			return processInfo.th32ProcessID;
		}
	}

	CloseHandle(processesSnapshot);
	return NULL;
}

int main()
{
	printf_s("Setting up new Mulligan Core dll...\n");

	TCHAR NPath[MAX_PATH];
	GetCurrentDirectory(MAX_PATH, NPath);

	string donwloadedFile = string(NPath) + string("\\HearthstoneMulliganNew.dll");
	string oldFile = string(NPath) + string("\\HearthstoneMulligan.dll");
	string smartBotPath = string(NPath) + string("\\SmartBotUI.exe");

	HANDLE sbProc, hsProc;
	DWORD sbExitCode, hsExitCode;
	killProc("SmartBotUI.exe", sbProc);
	killProc("Hearthstone.exe", hsProc);

	while (FindProcessId("SmartBotUI.exe") || FindProcessId("Hearthstone.exe"))
		Sleep(100);

	printf_s("Procs terminated\n");

	if (remove(oldFile.c_str()) != 0)
		perror("Error deleting file");
	else
	{
		while (exists(oldFile.c_str()))
			Sleep(100);

		printf_s("Old dll removed\n");

		puts("File successfully deleted\n");
		rename(donwloadedFile.c_str(), oldFile.c_str());

		startup(smartBotPath.c_str());
	}
	return 0;
}
