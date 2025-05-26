with Ada.Text_IO; use Ada.Text_IO;
with Ada.Numerics.Float_Random;

procedure Min_Finder is
   subtype Index_Type is Positive range 1 .. 1_000_000;
   type Int_Array is array(Index_Type) of Integer;
   A : Int_Array;

   use Ada.Numerics.Float_Random;
   G : Generator;

   protected Shared_Min is
      procedure Update_Min(Value : Integer; Index : Index_Type);
      function Get_Min return Integer;
      function Get_Index return Index_Type;
   private
      Min_Value : Integer := Integer'Last;
      Min_Index : Index_Type := 1;
   end Shared_Min;

   protected body Shared_Min is
      procedure Update_Min(Value : Integer; Index : Index_Type) is
      begin
         if Value < Min_Value then
            Min_Value := Value;
            Min_Index := Index;
         end if;
      end Update_Min;

      function Get_Min return Integer is
      begin
         return Min_Value;
      end Get_Min;

      function Get_Index return Index_Type is
      begin
         return Min_Index;
      end Get_Index;
   end Shared_Min;

   task type Worker(Start_Index, End_Index : Index_Type);

   task body Worker is
      Local_Min : Integer := Integer'Last;
      Local_Index : Index_Type := Start_Index;
   begin
      for I in Start_Index .. End_Index loop
         if A(I) < Local_Min then
            Local_Min := A(I);
            Local_Index := I;
         end if;
      end loop;
      Shared_Min.Update_Min(Local_Min, Local_Index);
   end Worker;

   Num_Tasks : constant := 4;
   Chunk_Size : constant Index_Type := Index_Type'Last / Num_Tasks;

   -- Створюємо кожну задачу вручну
   Task1 : Worker(1, Chunk_Size);
   Task2 : Worker(Chunk_Size + 1, Chunk_Size * 2);
   Task3 : Worker(Chunk_Size * 2 + 1, Chunk_Size * 3);
   Task4 : Worker(Chunk_Size * 3 + 1, Index_Type'Last);

begin
   Reset(G);
   for I in A'Range loop
      A(I) := Integer(Random(G) * 1000.0);
   end loop;
   A(500_000) := -100; -- спеціально вставлений мінімум

   -- Очікування завершення задач автоматичне

   Put_Line("Мінімальне значення: " & Integer'Image(Shared_Min.Get_Min));
   Put_Line("Індекс мінімального значення: " & Index_Type'Image(Shared_Min.Get_Index));
end Min_Finder;
