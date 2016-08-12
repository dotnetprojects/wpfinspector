#include "stdafx.h"
#include "hook.h"
#include <vcclr.h>

static unsigned int WM_INSPECT = ::RegisterWindowMessage(L"WM_INSPECT");
static HHOOK _messageHookHandle;

__declspec( dllexport ) 
int __stdcall MessageHookProc(int nCode, WPARAM wparam, LPARAM lparam) {

	if (nCode == HC_ACTION) 
	{
		CWPSTRUCT* msg = (CWPSTRUCT*)lparam;
		if (msg != NULL && msg->message == WM_INSPECT)
		{
			wchar_t* acmRemote = (wchar_t*)msg->wParam;

			System::String^ acmLocal = gcnew System::String(acmRemote);
			cli::array<System::String^>^ acmSplit = acmLocal->Split('$');

			System::Reflection::Assembly^ assembly = System::Reflection::Assembly::LoadFile(acmSplit[0]);
			if (assembly != nullptr)
			{
				System::Type^ type = assembly->GetType(acmSplit[1]);
				if (type != nullptr)
				{
					System::Reflection::MethodInfo^ methodInfo = type->GetMethod(acmSplit[2], System::Reflection::BindingFlags::Static | System::Reflection::BindingFlags::Public);
					if (methodInfo != nullptr)
					{
						methodInfo->Invoke(nullptr, nullptr);
					}
				}
			}
		}
	}
	return CallNextHookEx(_messageHookHandle, nCode, wparam, lparam);
}