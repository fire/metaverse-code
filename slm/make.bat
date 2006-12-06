@echo off
if "%1"=="clean" (
  del *.exe *.dll *.tmp
) else (
  echo making LayoutMgr.dll
  csc /nologo /target:library /out:LayoutMgr.dll GridLayout.cs ILayoutManager.cs LayoutControl.cs RubberLayout.cs QLayout.cs QConstrainedLayout.cs QConstrainedSpace.cs FlowLayout.cs

  echo making TestQ.exe
  csc /nologo /R:LayoutMgr.dll TestQ.cs

  echo making TestQConstrained.exe
  csc /nologo /R:LayoutMgr.dll TestQConstrained.cs

  echo making TestGrid.exe
  csc /nologo /R:LayoutMgr.dll TestGrid.cs

  echo making TestRubber.exe
  csc /nologo /R:LayoutMgr.dll TestRubber.cs

  echo making TestChLayout.exe
  csc /nologo /R:LayoutMgr.dll TestChLayout.cs

  echo making TestFlow.exe
  csc /nologo /R:LayoutMgr.dll TestFlow.cs

  echo making TestQConstrainedSpace.exe
  csc /nologo /R:LayoutMgr.dll TestQConstrainedSpace.cs
  
  echo making TestSplitChLayout.exe
  csc /nologo /R:LayoutMgr.dll TestSplitChLayout.cs
)
