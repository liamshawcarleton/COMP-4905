   M S B A M L   `   `   `  ����A  =Quarto, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:QuartoQuarto  5   	       Quarto.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   &localclr-namespace:Quarto     5   	   -   5          � Title$  
MainWindow��5   	   $	��500��6   $	��600��6)   
�������.��5	      � -   5        $��MainGrid��5	       � ColumnDefinitions 5
   
   �� 5      $��1* �6   �� 5      $	��296 �6   �� 5      $��1* �6    � RowDefinitions 5   
   �� 5      $	��296 �6   �� 5      $
��128* �6   �� 5      $
��auto �6   .+�5   
      Quarto.PlayerSlot  -   5          Name$ player1Slot��5      $��Center=�60   $��Center=�6K     5   
   -   5        $ player2Slot��5      $��Center=�60   $��Center=�6K   $��0��6h   $��2��6u      Quarto.GameBoard  5   
   -   5          Name$ 	mainBoard��5      $��1��6-      Quarto.PieceSlots  5   
   -   5          Name$ 
pieceSlots��5      $��1��6/   $��1��6<   � 5   
   -   5        $��TestingPanel��5      $��2��6(   $��3��65   $y�Hidden=�6I   .+�5      �� -   5        $��btnBinaryXOR��5      $��	BinaryXOR��6)   6=   �� 5      -	   5        $��btnSpotsRemaining��5      $��Spots Remaining��6.   6H   �� 5      -
   5        $��btnEvaluate��5      $��Evaluate Board��6(   6A   5      � 5   
   -   5        $��PlayingPanel��5      $��2��6(   $��3��65   $y�Visible=�6I   .+�5      �� -   5        $��btnEasy��5      $��Easy AI��6$   66   �� 5      -   5        $��	btnMedium��5      $��	Medium AI��6&   6:   �� 5       -   5        $��btnHard��5       $��Hard AI��6$   66   5!      5"      5#      