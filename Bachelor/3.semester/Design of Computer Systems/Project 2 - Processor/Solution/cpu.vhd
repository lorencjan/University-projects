-- cpu.vhd: Simple 8-bit CPU (BrainF*ck interpreter)
-- Copyright (C) 2019 Brno University of Technology,
--                    Faculty of Information Technology
-- Author(s): Jan Lorenc (xloren15)
--

library ieee;
use ieee.std_logic_1164.all;
use ieee.std_logic_arith.all;
use ieee.std_logic_unsigned.all;

-- ----------------------------------------------------------------------------
--                        Entity declaration
-- ----------------------------------------------------------------------------
entity cpu is
 port (
   CLK   : in std_logic;  -- hodinovy signal
   RESET : in std_logic;  -- asynchronni reset procesoru
   EN    : in std_logic;  -- povoleni cinnosti procesoru
 
   -- synchronni pamet RAM
   DATA_ADDR  : out std_logic_vector(12 downto 0); -- adresa do pameti
   DATA_WDATA : out std_logic_vector(7 downto 0); -- mem[DATA_ADDR] <- DATA_WDATA pokud DATA_EN='1'
   DATA_RDATA : in std_logic_vector(7 downto 0);  -- DATA_RDATA <- ram[DATA_ADDR] pokud DATA_EN='1'
   DATA_RDWR  : out std_logic;                    -- cteni (0) / zapis (1)
   DATA_EN    : out std_logic;                    -- povoleni cinnosti
   
   -- vstupni port
   IN_DATA   : in std_logic_vector(7 downto 0);   -- IN_DATA <- stav klavesnice pokud IN_VLD='1' a IN_REQ='1'
   IN_VLD    : in std_logic;                      -- data platna
   IN_REQ    : out std_logic;                     -- pozadavek na vstup data
   
   -- vystupni port
   OUT_DATA : out  std_logic_vector(7 downto 0);  -- zapisovana data
   OUT_BUSY : in std_logic;                       -- LCD je zaneprazdnen (1), nelze zapisovat
   OUT_WE   : out std_logic                       -- LCD <- OUT_DATA pokud OUT_WE='1' a OUT_BUSY='0'
 );
end cpu;


-- ----------------------------------------------------------------------------
--                      Architecture declaration
-- ----------------------------------------------------------------------------
architecture behavioral of cpu is

  -- programovy citac (PC)
  signal pc_reg: std_logic_vector(12 downto 0);
  signal pc_inc: std_logic;
  signal pc_dec: std_logic;

  -- ukazatel do pameti (PTR)
  signal mem_ptr: std_logic_vector(12 downto 0);
  signal mem_ptr_inc: std_logic;
  signal mem_ptr_dec: std_logic;
  
  -- citac cyklu (CNT)
  signal cnt: std_logic_vector(7 downto 0);
  signal cnt_inc: std_logic;
  signal cnt_dec: std_logic;

  -- adresa pomocne promenna
  signal tmp: std_logic_vector(12 downto 0);

  -- signaly a stavy KA
  type fsm_state is (idle_s, fetch_s, decode_s, inc_ptr_s, dec_ptr_s,
                     inc_val_1_s, inc_val_2_s, dec_val_1_s, dec_val_2_s,
                     while_start_1_s, while_start_2_s, while_start_3_s, while_start_4_s, while_start_5_s,
                     while_end_1_s, while_end_2_s, while_end_3_s, while_end_4_s, while_end_5_s,
                     write_1_s, write_2_s, read_1_s, read_2_s,
                     store_to_tmp_1_s, store_to_tmp_2_s, load_from_tmp_1_s, load_from_tmp_2_s, null_s, nop_s);
  signal p_state: fsm_state;  --aktualni stav
  signal n_state: fsm_state;  --nasledujici stav

  -- prepinace multiplexoru
  signal mx_address: std_logic_vector(1 downto 0);
  signal mx_wdata: std_logic_vector(1 downto 0);
begin

  -- zde dopiste vlastni VHDL kod

  -- programovy citac ... zaklad vychazi ze cviceni, jen doplnena dekrementace
  pc_process: process(RESET, CLK)
  begin
    if(RESET='1') then
      pc_reg <= (others=>'0');
    elsif rising_edge(CLK) then
      if(pc_inc='1') then
        pc_reg <= pc_reg + 1;
      elsif(pc_dec='1') then
        pc_reg <= pc_reg - 1;
      end if;
    end if;
  end process pc_process;
  
  -- ukazatel do pameti ... stejne jako PC, jen inicializace na 0x1000
  mem_ptr_process: process(RESET, CLK)
  begin
    if(RESET='1') then
      mem_ptr <= (12 => '1', others => '0');
    elsif rising_edge(CLK) then
      if(mem_ptr_inc='1') then
        mem_ptr <= mem_ptr + 1;
      elsif(mem_ptr_dec='1') then
        mem_ptr <= mem_ptr - 1;
      end if;
    end if;
  end process mem_ptr_process;

  -- citac cyklu ... opet stejne jako PC
  while_cntr_process: process(RESET, CLK)
  begin
    if(RESET='1') then
      cnt <= (others => '0');
    elsif rising_edge(CLK) then
      if(cnt_inc = '1') then
        cnt <= cnt + 1;
      elsif(cnt_dec = '1') then
        cnt <= cnt - 1;
      end if;
    end if;
  end process while_cntr_process;

  -- proces na inicializaci tmp na 0x1000 ...  inicializoval bych pri deklaraci, ale to hazi warning, nechci prijit o body :P
  tmp_init: process(RESET)
  begin
    if(RESET='1') then
        tmp <= (12 => '1', others => '0');
    end if;
  end process tmp_init;

  -- aktualni stav KA
  fsm_pstate: process(RESET, CLK)
  begin
    if(RESET='1') then
      p_state <= idle_s;
    elsif rising_edge(CLK) and (EN = '1') then
      p_state <= n_state;
    end if;
  end process fsm_pstate;

  -- nasledujici stav KA
  fsm_nstate: process(DATA_RDATA, IN_VLD, OUT_BUSY, p_state, cnt, mx_wdata)
  begin
    -- inicializujeme/nastavime vychozi hodnoty promennym
    -- porty
    DATA_RDWR <= '0';
    DATA_EN   <= '0';
    IN_REQ    <= '0';
    OUT_WE    <= '0';
    -- signaly
    pc_inc      <= '0';
    pc_dec      <= '0';
    mem_ptr_inc <= '0';
    mem_ptr_dec <= '0';
    cnt_inc     <= '0';
    cnt_dec     <= '0';
    -- mx selektory, nutno inicializovat kvuli latchi
    mx_address  <= "00";
    mx_wdata    <= "00";

    -- stavovy prepinac
    case p_state is
      --idle
      when idle_s => n_state <= fetch_s;

      --fetch
      when fetch_s =>
        DATA_EN <= '1';
        mx_address <= "00";   --nacitam instrukci
        n_state <= decode_s;

      --decode
      when decode_s =>
        case DATA_RDATA is
          when x"3E" => n_state <= inc_ptr_s;
          when x"3C" => n_state <= dec_ptr_s;
          when x"2B" => n_state <= inc_val_1_s;
          when x"2D" => n_state <= dec_val_1_s;
          when x"5B" => n_state <= while_start_1_s;
          when x"5D" => n_state <= while_end_1_s;
          when x"2E" => n_state <= write_1_s;
          when x"2C" => n_state <= read_1_s;
          when x"24" => n_state <= store_to_tmp_1_s;
          when x"21" => n_state <= load_from_tmp_1_s;
          when x"00" => n_state <= null_s;
          when others => n_state <= nop_s;
        end case;

      -- /* jednotlive instrukce *\
      -- (de)inkrementace ukazatele do pameti
      when inc_ptr_s =>
        pc_inc <= '1';
        mem_ptr_inc <= '1';
        n_state <= fetch_s;
      when dec_ptr_s =>
        pc_inc <= '1';
        mem_ptr_dec <= '1';
        n_state <= fetch_s;

      -- inkrementace hodnoty v pameti
      when inc_val_1_s =>   -- *** je treba na dvakrat, nejprve nacteme data ***
        DATA_EN <= '1';     --povolit pristup
        DATA_RDWR <= '0';   --precist ... ulozeno v DATA_RDATA
        mx_address <= "01"; --nacitame data
        n_state <= inc_val_2_s;
      when inc_val_2_s =>   -- *** a pak zapiseme inkrementovane ***
        pc_inc <= '1';
        DATA_EN <= '1';     --povolit pristup
        DATA_RDWR <= '1';   --nastavit zapis
        mx_address <= "01"; --zapisujeme data
        mx_wdata <= "10";   --inkrementovane o 1
        n_state <= fetch_s;

      -- dekrementace hodnoty v pameti - identicke s INC, pouze nastavime na multiplexoru dekrementaci
      when dec_val_1_s =>
        DATA_EN <= '1';
        DATA_RDWR <= '0';
        mx_address <= "01";
        n_state <= dec_val_2_s;
      when dec_val_2_s =>
        pc_inc <= '1';
        DATA_EN <= '1';
        DATA_RDWR <= '1';
        mx_address <= "01";
        mx_wdata <= "01";  -- dekrementovane o 1
        n_state <= fetch_s;
      
      -- smycka, nejslozitejsi instrukce, na nekolik kroku, pekne si odsimulujeme pseudokod ze zadani
      -- zacatek '['
      when while_start_1_s =>
        DATA_EN <= '1';    -- precteme data
        DATA_RDWR <= '0';  -- ktere budeme v IFu kontrolovat s nulou
        mx_address <= "01";
        pc_inc <= '1';     -- PC <- PC + 1
        n_state <= while_start_2_s;
      when while_start_2_s =>
        if(DATA_RDATA(7 downto 0) = "00000000") then -- if (mem[PTR] == 0)
          cnt_inc <= '1';                            -- CNT <- 1
          n_state <= while_start_3_s;    -- while (CNT != 0)  ...inkrementovali jsme CNT, vime, ze podminka je splnena, jdeme do cyklu
        else
          n_state <= fetch_s;            -- jestli podminka neni splnena, pokracujeme dalsi instrukci
        end if;
      when while_start_3_s =>  -- de facto se jedna o fetch
        DATA_EN <= '1';        -- c <- mem[PC]
        mx_address <= "00";
        n_state <= while_start_4_s;
      when while_start_4_s =>
        if(DATA_RDATA = x"5B") then     -- if (c == '[')
          cnt_inc <= '1';               -- CNT <- CNT + 1
        elsif(DATA_RDATA = x"5D") then  -- elsif (c == ']')
          cnt_dec <= '1';               -- CNT <- CNT - 1
        end if;
        pc_inc <= '1';                  -- PC <- PC + 1
        n_state <= while_start_5_s;
      when while_start_5_s =>
        if(cnt(7 downto 0) = "00000000") then -- pokud je citac nulovy
          n_state <= fetch_s;                 -- jsme venku
        else                                  -- jinak pokracovat v cyklu
          n_state <= while_start_3_s;         -- (overeni while (CNT != 0))
        end if;
      -- konec ']' ... stejne jako '[', ale obracene, opet simulujeme pseudokod ze zadani
      when while_end_1_s =>
        DATA_EN <= '1';    -- precteme data
        DATA_RDWR <= '0';  -- ktere budeme v IFu kontrolovat s nulou
        mx_address <= "01";
        n_state <= while_end_2_s;
      when while_end_2_s =>
        if(DATA_RDATA(7 downto 0) = "00000000") then  -- if (mem[PTR] == 0)
          pc_inc <= '1';                              -- PC <- PC + 1
          n_state <= fetch_s;
        else                                          -- else
          cnt_inc <= '1';                             -- CNT <- 1
          pc_dec <= '1';                              -- PC <- PC - 1
          n_state <= while_end_3_s;
        end if;
      when while_end_3_s =>    -- opet de facto fetch
        DATA_EN <= '1';        -- c <- mem[PC]
        mx_address <= "00";
        n_state <= while_end_4_s;
      when while_end_4_s =>
        if(DATA_RDATA = x"5D") then     -- if (c == ']')
          cnt_inc <= '1';               -- CNT <- CNT + 1
        elsif(DATA_RDATA = x"5B") then  -- elsif (c == '[')
          cnt_dec <= '1';               -- CNT <- CNT - 1
        end if;
        n_state <= while_end_5_s;
      when while_end_5_s =>
        if(cnt(7 downto 0) = "00000000") then  -- if (CNT == 0)
          pc_inc <= '1';                       -- PC <- PC + 1
          n_state <= fetch_s;
        else
          pc_dec <= '1';                       -- PC <- PC - 1 
          n_state <= while_end_3_s;
        end if;
      
      -- vypis hodnoty - podobne jako u inkrementace hodnoty tak na 2 pruchody, nejprve precist, potom nechat vypsat
      when write_1_s =>
        DATA_EN <= '1';
        DATA_RDWR <= '0';
        mx_address <= "01";
        n_state <= write_2_s;
      when write_2_s =>
        if(OUT_BUSY = '0') then   --zapisujeme jenom, pokud neni lcd zaneprazdnen
          pc_inc <= '1';
          OUT_WE <= '1';          --povolime vypis ... (OUT_DATA <= DATA_RDATA implementovano na konci souboru, tu by generovalo latch)
          n_state <= fetch_s;
        else
          n_state <= write_2_s;   --pokud je lcd zaneprazdnen, cekam, dokud se neuvolni ... while (OUT BUSY) {}
        end if;
        
      -- nacteni hodnoty - stejne jako zapis, jen se vstupnim portem
      when read_1_s =>
        IN_REQ <= '1';        --pozadam o cteni
        n_state <= read_2_s;
      when read_2_s =>
        if(IN_VLD = '1') then --ctu pouze, pokud mi neco doslo
          pc_inc <= '1';
          DATA_EN <= '1';     --a zapisu prectenou hodnotu do pameti, stejne jako (de)inkrementace - povolit
          DATA_RDWR <= '1';   --nastavit zapis
          mx_address <= "01";
          mx_wdata <= "00";
          IN_REQ <= '0';      --nesmim zapomenout ukoncit pozadavek na cteni
          n_state <= fetch_s;
        else
          IN_REQ <= '1';      --pokud nic nedoslo, pokracuju ve cteni
          n_state <= read_2_s;--podobne jako u zapisu, pokud nic nedoslo, cekam, nez nedojde ... while (!IN VLD) {}
        end if;

      -- ulozeni do pomocne promenne ... opet na dva pruchody
      when store_to_tmp_1_s =>  --nacteme z pameti
        DATA_EN <= '1';         -- DATA RDATA <- mem[PTR] ...predpokladam tu byla chyba v zadani (nacitam DO tmp, ne Z tmp)
        DATA_RDWR <= '0';
        mx_address <= "01";
        n_state <= store_to_tmp_2_s;
      when store_to_tmp_2_s => --zapiseme do pameti na adrese tmp
        DATA_EN <= '1';
        DATA_RDWR <= '1';
        mx_address <= "10";    -- nastavim adresu tmp
        mx_wdata <= "11";      -- mem[TMP] <- DATA RDATA
        pc_inc <= '1';         -- PC <- PC + 1
        n_state <= fetch_s;

      -- ulozeni z pomocne promenne do pameti ... opak store_to_tmp, jinak stejne
      when load_from_tmp_1_s =>
        DATA_EN <= '1';
        DATA_RDWR <= '0';
        mx_address <= "10"; -- DATA RDATA <- mem[TMP]
        n_state <= load_from_tmp_2_s;
      when load_from_tmp_2_s =>
        DATA_EN <= '1';
        DATA_RDWR <= '1';   -- mem[PTR] <- DATA RDATA
        mx_address <= "01";
        mx_wdata <= "11";
        pc_inc <= '1';      -- PC <- PC + 1
        n_state <= fetch_s;

      -- zastaveni programu, v tomto stavu se jiz zustava
      when null_s => n_state <= null_s;

      -- pokud je stav nedefinovany, zvysime programovy citac a cteme dalsi instrukci
      when others =>
        pc_inc <= '1';
        n_state <= fetch_s;
    end case;
  end process fsm_nstate;
  
  -- multiplexory podle obrazku ze zadani
  -- adresa dat vs adresa programu vs adresa tmp (kvuli tmp je treba mx rozsirit)
  MX1: process(mx_address, pc_reg, mem_ptr, tmp)
  begin
    DATA_ADDR <= pc_reg; --ochrana proti latch
    case mx_address is
      when "00" => DATA_ADDR <= pc_reg;
      when "01" => DATA_ADDR <= mem_ptr;
      when "10" => DATA_ADDR <= tmp;
      when others =>
    end case;
  end process MX1;
  -- MX2 vynechavam, jedna z vyberovych hodnot je vychozi a nevede do toho zadny 'sel', nepotrebuji ho
  -- zapisova hodnota
  MX3: process(mx_wdata, IN_DATA, DATA_RDATA, tmp)
  begin
    DATA_WDATA <= DATA_RDATA; --ochrana proti latch
    case mx_wdata is
      when "00" => DATA_WDATA <= IN_DATA;
      when "01" => DATA_WDATA <= DATA_RDATA - 1;
      when "10" => DATA_WDATA <= DATA_RDATA + 1;
      when "11" => DATA_WDATA <= DATA_RDATA;  -- z tmp
      when others =>
    end case;
  end process MX3;
  
  -- kombinacne fixni prirazeni OUT_DATA ... puvodne ve stavu write_2_s v podmince s OUT_BUSY, ale to hazi latch warning
  OUT_DATA <= DATA_RDATA;
 
end behavioral;
 
