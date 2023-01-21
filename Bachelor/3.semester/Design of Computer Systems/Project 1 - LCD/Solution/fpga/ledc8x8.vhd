-- Autor reseni: Jan Lorenc, xloren15

library IEEE;
use IEEE.std_logic_1164.all;
use IEEE.std_logic_arith.all;
use IEEE.std_logic_unsigned.all;

entity ledc8x8 is
port ( -- Sem doplnte popis rozhrani obvodu.
    SMCLK, RESET: in std_logic;
    ROW, LED: out std_logic_vector(0 to 7)
);
end ledc8x8;

architecture main of ledc8x8 is

    -- Sem doplnte definice vnitrnich signalu.
    signal ce: std_logic := '0';                 -- clock enable
    signal wait_for_half_sec: std_logic := '0';  -- je-li v '1', tak cekame a lcd je neaktivni
    signal check_cnt: std_logic := '1';          -- de facto counter enable, je-li aktivni, pocitame pulvterinu, jinak necitame
    
    signal ce_cnt, lcd_leds, lcd_rows: std_logic_vector(7 downto 0);
    -- pocita, kolikrat se projel cely row ... 7.3728MHz / 256 = 1/28800s frekvence SMLK
    -- cely row ma f = 8*f(smlk) = 1/3600s  -> 0.5s = 1800/3600  -> jakmile se projede 1800x row, cekame
    signal row_cnt: std_logic_vector(10 downto 0) := (others => '0'); -- 1800 = 11100001000(bin)

begin

    -- Sem doplnte popis obvodu. Doporuceni: pouzivejte zakladni obvodove prvky
    -- (multiplexory, registry, dekodery,...), jejich funkce popisujte pomoci
    -- procesu VHDL a propojeni techto prvku, tj. komunikaci mezi procesy,
    -- realizujte pomoci vnitrnich signalu deklarovanych vyse.

    -- DODRZUJTE ZASADY PSANI SYNTETIZOVATELNEHO VHDL KODU OBVODOVYCH PRVKU,
    -- JEZ JSOU PROBIRANY ZEJMENA NA UVODNICH CVICENI INP A SHRNUTY NA WEBU:
    -- http://merlin.fit.vutbr.cz/FITkit/docs/navody/synth_templates.html.

    -- Nezapomente take doplnit mapovani signalu rozhrani na piny FPGA
    -- v souboru ledc8x8.ucf.

    -- citac z prezentace o fitkitu (prednaska 3) na snizeni frekvence
    -- doplneny o signal reset
	ce_gen: process(SMCLK, RESET)
	begin
		if RESET = '1' then
			ce_cnt <= (others => '0');
		elsif SMCLK'event and SMCLK = '1' then
			ce_cnt <= ce_cnt + 1;
		end if;
	end process ce_gen;
    ce <= '1' when ce_cnt(7 downto 0) = "11111111" else '0';

    -- posuvny registr v prevzany z fitkit stranek s prejmenovanymi promennymi
    -- doplneno o citac radku viz. definice 'row_cnt'
    rot_reg: process(RESET, SMCLK)
    begin
        if (RESET='1') then
            lcd_rows <= (7 => '1', others => '0');
        elsif (SMCLK'event) and (SMCLK='1') and (ce='1') then
            lcd_rows <= lcd_rows(0) & lcd_rows(7 downto 1);
            --kontrola cekani
            if(check_cnt = '1') and lcd_rows = "1000000" then --pokud mame jeste vubec na neco cekat, pocitame pruchody radku
                if(row_cnt = "11100001000") then                 
                    row_cnt <= (others => '0'); -- jinak vynuluj citac
                    if(wait_for_half_sec = '0') then -- a pokud se citala prvni pulvterina, kdy je lcd aktivni
                        wait_for_half_sec <= '1';    -- tak ho zneaktivnime a citame dalsi pulvterinu
                    else
                        wait_for_half_sec <= '0';    -- pokud je to druhe citani, tedy 'wait_for_half_sec' bylo aktivni (=1)
                        check_cnt <= '0';            -- tak cele citani ukoncime a jede se jako by tu tento blok nebyl
                    end if;
                else --dokud se nedosahlo pulvteriny, pricitej
                    row_cnt <= row_cnt + 1;
                end if;
            end if;
        end if;
    end process rot_reg;
    
    -- namapovani radku s ledkama dle inicialu JL (Jan Lorenc)
    -- pokud se ceka, generuje jenom jednicky -> nesviti se
    map_lcd: process(lcd_rows)
    begin
        if(wait_for_half_sec = '0') then
            case lcd_rows is
                when "10000000" => lcd_leds <= "11011111";
                when "01000000" => lcd_leds <= "11011111";
                when "00100000" => lcd_leds <= "11011111";
                when "00010000" => lcd_leds <= "01010111";
                when "00001000" => lcd_leds <= "00010111";
                when "00000100" => lcd_leds <= "11110111";
                when "00000010" => lcd_leds <= "11110111";
                when "00000001" => lcd_leds <= "11110000";
                when others     => lcd_leds <= "11111111";
            end case;
        else
            lcd_leds <= (others => '1');
        end if;
    end process map_lcd;

    -- poslani signalu
    ROW <= lcd_rows;
    LED <= lcd_leds;

end main;




-- ISID: 75579
