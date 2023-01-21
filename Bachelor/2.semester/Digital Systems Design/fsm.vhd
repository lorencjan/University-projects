-- fsm.vhd: Finite State Machine
-- Author(s): Jan Lorenc
--
library ieee;
use ieee.std_logic_1164.all;
-- ----------------------------------------------------------------------------
--                        Entity declaration
-- ----------------------------------------------------------------------------
entity fsm is
port(
   CLK         : in  std_logic;
   RESET       : in  std_logic;

   -- Input signals
   KEY         : in  std_logic_vector(15 downto 0);
   CNT_OF      : in  std_logic;

   -- Output signals
   FSM_CNT_CE  : out std_logic;
   FSM_MX_MEM  : out std_logic;
   FSM_MX_LCD  : out std_logic;
   FSM_LCD_WR  : out std_logic;
   FSM_LCD_CLR : out std_logic
);
end entity fsm;

-- ----------------------------------------------------------------------------
--                      Architecture declaration
-- ----------------------------------------------------------------------------
architecture behavioral of fsm is
   type t_state is (START, COMMON1, COMMON2, COMMON3, COMMON4,
                    FIRST5, FIRST6, FIRST7, FIRST8, FIRST9, FIRST10,
                    SECOND5, SECOND6, SECOND7, SECOND8, SECOND9, SECOND10,
                    PRINT_CORRECT, PRINT_WRONG, WRONG_INPUT, FINISH);
   signal present_state, next_state : t_state;

begin
-- -------------------------------------------------------
sync_logic : process(RESET, CLK)
begin
   if (RESET = '1') then
      present_state <= START;
   elsif (CLK'event AND CLK = '1') then
      present_state <= next_state;
   end if;
end process sync_logic;

-- -------------------------------------------------------
next_state_logic : process(present_state, KEY, CNT_OF)
begin
   case (present_state) is
   -- - - - - - - - - - - - - - - - - - - - - - -
   -- SPOLECNA CAST KODU
   when START =>
      next_state <= START;
      if (KEY(15) = '1') then
         next_state <= PRINT_WRONG;
      elsif (KEY(1) = '1') then
         next_state <= COMMON1;
      elsif (KEY(14 downto 0) /= "000000000000000") then
         next_state <= WRONG_INPUT;
      end if;
   -- - - - - - - - - - - - - - - - - - - - - - -
   when COMMON1 =>
      next_state <= COMMON1;
      if (KEY(15) = '1') then
         next_state <= PRINT_WRONG;
      elsif (KEY(0) = '1') then
         next_state <= COMMON2;
      elsif (KEY(14 downto 0) /= "000000000000000") then
         next_state <= WRONG_INPUT;
      end if;
   -- - - - - - - - - - - - - - - - - - - - - - -
   when COMMON2 =>
      next_state <= COMMON2;
      if (KEY(15) = '1') then
         next_state <= PRINT_WRONG;
      elsif (KEY(8) = '1') then
         next_state <= COMMON3;
      elsif (KEY(14 downto 0) /= "000000000000000") then
         next_state <= WRONG_INPUT;
      end if;
   -- - - - - - - - - - - - - - - - - - - - - - -
   when COMMON3 =>
      next_state <= COMMON3;
      if (KEY(15) = '1') then
         next_state <= PRINT_WRONG;
      elsif (KEY(3) = '1') then
         next_state <= COMMON4;
      elsif (KEY(14 downto 0) /= "000000000000000") then
         next_state <= WRONG_INPUT;
      end if;
   -- - - - - - - - - - - - - - - - - - - - - - -
   when COMMON4 =>
      next_state <= COMMON4;
      if (KEY(15) = '1') then
         next_state <= PRINT_WRONG;
      elsif (KEY(6) = '1') then
         next_state <= FIRST5;
      elsif (KEY(8) = '1') then
         next_state <= SECOND5;
      elsif (KEY(14 downto 0) /= "000000000000000") then
         next_state <= WRONG_INPUT;
      end if;
   -- - - - - - - - - - - - - - - - - - - - - - -
   -- VETEV PRVNIHO KODU
   when FIRST5 =>
      next_state <= FIRST5;
      if (KEY(15) = '1') then
         next_state <= PRINT_WRONG;
      elsif (KEY(8) = '1') then
         next_state <= FIRST6;
      elsif (KEY(14 downto 0) /= "000000000000000") then
         next_state <= WRONG_INPUT;
      end if;
   -- - - - - - - - - - - - - - - - - - - - - - -
   when FIRST6 =>
      next_state <= FIRST6;
      if (KEY(15) = '1') then
         next_state <= PRINT_WRONG;
      elsif (KEY(7) = '1') then
         next_state <= FIRST7;
      elsif (KEY(14 downto 0) /= "000000000000000") then
         next_state <= WRONG_INPUT;
      end if;
   -- - - - - - - - - - - - - - - - - - - - - - -
   when FIRST7 =>
      next_state <= FIRST7;
      if (KEY(15) = '1') then
         next_state <= PRINT_WRONG;
      elsif (KEY(3) = '1') then
         next_state <= FIRST8;
      elsif (KEY(14 downto 0) /= "000000000000000") then
         next_state <= WRONG_INPUT;
      end if;
   -- - - - - - - - - - - - - - - - - - - - - - -
   when FIRST8 =>
      next_state <= FIRST8;
      if (KEY(15) = '1') then
         next_state <= PRINT_WRONG;
      elsif (KEY(0) = '1') then
         next_state <= FIRST9;
      elsif (KEY(14 downto 0) /= "000000000000000") then
         next_state <= WRONG_INPUT;
      end if;
   -- - - - - - - - - - - - - - - - - - - - - - -
   when FIRST9 =>
      next_state <= FIRST9;
      if (KEY(15) = '1') then
         next_state <= PRINT_WRONG;
      elsif (KEY(9) = '1') then
         next_state <= FIRST10;
      elsif (KEY(14 downto 0) /= "000000000000000") then
         next_state <= WRONG_INPUT;
      end if;
   -- - - - - - - - - - - - - - - - - - - - - - -
   when FIRST10 =>
      next_state <= FIRST10;
      if (KEY(15) = '1') then
         next_state <= PRINT_CORRECT;
      elsif (KEY(14 downto 0) /= "000000000000000") then
         next_state <= WRONG_INPUT;
      end if;
   -- - - - - - - - - - - - - - - - - - - - - - -
   -- VETEV DRUHEHO KODU
   when SECOND5 =>
      next_state <= SECOND5;
      if (KEY(15) = '1') then
         next_state <= PRINT_WRONG;
      elsif (KEY(1) = '1') then
         next_state <= SECOND6;
      elsif (KEY(14 downto 0) /= "000000000000000") then
         next_state <= WRONG_INPUT;
      end if;
   -- - - - - - - - - - - - - - - - - - - - - - -
   when SECOND6 =>
      next_state <= SECOND6;
      if (KEY(15) = '1') then
         next_state <= PRINT_WRONG;
      elsif (KEY(1) = '1') then
         next_state <= SECOND7;
      elsif (KEY(14 downto 0) /= "000000000000000") then
         next_state <= WRONG_INPUT;
      end if;
   -- - - - - - - - - - - - - - - - - - - - - - -
   when SECOND7 =>
      next_state <= SECOND7;
      if (KEY(15) = '1') then
         next_state <= PRINT_WRONG;
      elsif (KEY(1) = '1') then
         next_state <= SECOND8;
      elsif (KEY(14 downto 0) /= "000000000000000") then
         next_state <= WRONG_INPUT;
      end if;
   -- - - - - - - - - - - - - - - - - - - - - - -
   when SECOND8 =>
      next_state <= SECOND8;
      if (KEY(15) = '1') then
         next_state <= PRINT_WRONG;
      elsif (KEY(6) = '1') then
         next_state <= SECOND9;
      elsif (KEY(14 downto 0) /= "000000000000000") then
         next_state <= WRONG_INPUT;
      end if;
   -- - - - - - - - - - - - - - - - - - - - - - -
   when SECOND9 =>
      next_state <= SECOND9;
      if (KEY(15) = '1') then
         next_state <= PRINT_WRONG;
      elsif (KEY(8) = '1') then
         next_state <= SECOND10;
      elsif (KEY(14 downto 0) /= "000000000000000") then
         next_state <= WRONG_INPUT;
      end if;
   -- - - - - - - - - - - - - - - - - - - - - - -
   when SECOND10 =>
      next_state <= SECOND10;
      if (KEY(15) = '1') then
         next_state <= PRINT_CORRECT;
      elsif (KEY(14 downto 0) /= "000000000000000") then
         next_state <= WRONG_INPUT;
      end if;
   -- - - - - - - - - - - - - - - - - - - - - - -
   -- VYSLEDNE STAVY
   when PRINT_CORRECT =>
      next_state <= PRINT_CORRECT;
      if (CNT_OF = '1') then
         next_state <= FINISH;
      end if;
   -- - - - - - - - - - - - - - - - - - - - - - -
   when WRONG_INPUT =>
      next_state <= WRONG_INPUT;
      if (KEY(15) = '1') then
         next_state <= PRINT_WRONG;
      elsif (KEY(14 downto 0) /= "000000000000000") then
         next_state <= WRONG_INPUT;
      end if;
   -- - - - - - - - - - - - - - - - - - - - - - -
   when PRINT_WRONG =>
      next_state <= PRINT_WRONG;
      if (CNT_OF = '1') then
         next_state <= FINISH;
      end if;
   -- - - - - - - - - - - - - - - - - - - - - - -
   when FINISH =>
      next_state <= FINISH;
      if (KEY(15) = '1') then
         next_state <= START; 
      end if;
   -- - - - - - - - - - - - - - - - - - - - - - -
   when others =>
      next_state <= START;
   end case;
end process next_state_logic;

-- -------------------------------------------------------
output_logic : process(present_state, KEY)
begin
   FSM_CNT_CE     <= '0';
   FSM_MX_MEM     <= '0';
   FSM_MX_LCD     <= '0';
   FSM_LCD_WR     <= '0';
   FSM_LCD_CLR    <= '0';

   case (present_state) is
   -- - - - - - - - - - - - - - - - - - - - - - -
   when PRINT_CORRECT =>
      FSM_CNT_CE     <= '1';
      FSM_MX_MEM     <= '1'; -- nastavi zpravu uspechu
      FSM_MX_LCD     <= '1';
      FSM_LCD_WR     <= '1';
   -- - - - - - - - - - - - - - - - - - - - - - -
   when PRINT_WRONG =>
      FSM_CNT_CE     <= '1';
      FSM_MX_MEM     <= '0'; -- nastavi negativni zpravu
      FSM_MX_LCD     <= '1';
      FSM_LCD_WR     <= '1';
   -- - - - - - - - - - - - - - - - - - - - - - -
   when FINISH =>
      if (KEY(15) = '1') then
         FSM_LCD_CLR    <= '1';
      end if;
   -- - - - - - - - - - - - - - - - - - - - - - -
   when others =>
      if (KEY(14 downto 0) /= "000000000000000") then
         FSM_LCD_WR     <= '1';
      end if;
      if (KEY(15) = '1') then
         FSM_LCD_CLR    <= '1';
      end if;
   end case;
end process output_logic;

end architecture behavioral;

