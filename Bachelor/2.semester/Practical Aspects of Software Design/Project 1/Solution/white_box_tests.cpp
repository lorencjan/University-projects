//======== Copyright (c) 2017, FIT VUT Brno, All rights reserved. ============//
//
// Purpose:     White Box - Tests suite
//
// $NoKeywords: $ivs_project_1 $white_box_code.cpp
// $Author:     JAN LORENC <xloren15@stud.fit.vutbr.cz>
// $Date:       $2019-02-12
//============================================================================//
/**
 * @file white_box_tests.cpp
 * @author JAN LORENC
 *
 * @brief Implementace testu prace s maticemi.
 */

#include "gtest/gtest.h"
#include "white_box_code.h"

 //============================================================================//
 // ** ZDE DOPLNTE TESTY **
 //
 // Zde doplnte testy operaci nad maticemi. Cilem testovani je:
 // 1. Dosahnout maximalniho pokryti kodu (white_box_code.cpp) testy.
 // 2. Overit spravne chovani operaci nad maticemi v zavislosti na rozmerech 
 //    matic.
 //============================================================================//

using namespace std;

/*testy konstruktoru*/
TEST(ConstructorTests, DefaultConstructor)
{
	Matrix matrix;						 //vytvorim matici 1x1
	EXPECT_TRUE(matrix.set(0, 0, 5.0));	 //naplnim hodnotou 5
	EXPECT_EQ(matrix.get(0, 0), 5.0);	 //mela by tam byt
	ASSERT_FALSE(matrix.set(1, 0, 5.0)); //zapisi mimo -> melo by vratit false
}
TEST(ConstructorTests, OverloadedConstructor)
{
	//vyzkousim vyjimku
	EXPECT_ANY_THROW(Matrix(0, 0));

	//testuji matici 4x4
	const int rows = 4, cols = 4;
	Matrix matrix(rows, cols);
	for (int i = 0; i < rows; i++)
		for (int j = 0; j < cols; j++)
			EXPECT_TRUE(matrix.set(i, j, 1));

	//zapisi mimo -> melo by vratit false
	ASSERT_FALSE(matrix.set(rows, cols, 5.0));
}

/*testy getteru a setteru*/
//fixture trida
class GetSetTest : public ::testing::Test
{
protected:
	Matrix *matrix;
	const int rows = 5, cols = 5;

	virtual void SetUp()
	{
		matrix = new Matrix(rows, cols);
	}
	virtual void TearDown() override
	{
		delete matrix;
	}
};
//setter testy jsou podobne na pretizeny konstruktor, tam vsak byl duraz
//na meze, ktere se settery testovaly, tu jde o settery samotne
TEST_F(GetSetTest, BasicSetter)
{
	//pokusim se naplnit jednickami mimo meze -> nemelo by to jit -> false
	for (int i = rows; i < 2 * rows; i++)
		for (int j = cols; j < 2 * cols; j++)
			EXPECT_FALSE(matrix->set(i, j, 1));

	//naplnim hodnotami 0-24
	for (int i = 0; i < rows; i++)
		for (int j = 0; j < cols; j++)
			ASSERT_TRUE(matrix->set(i, j, i*cols + j));

	//vyzkousim par nahodnych hodnot
	ASSERT_EQ(matrix->get(2, 3), 13.0);	 // 3.rad., 4.sl -> 2*5+3 = 13
	ASSERT_EQ(matrix->get(1, 4), 9.0);	 // 2.rad., 5.sl -> 1*5+4 = 9
	ASSERT_EQ(matrix->get(0, 1), 1.0);	 // 1.rad., 2.sl -> 0*5+1 = 1
	ASSERT_EQ(matrix->get(3, 0), 15.0);	 // 4.rad., 1.sl -> 3*5+0 = 15
}
TEST_F(GetSetTest, VectorSetter)
{
	//vytvorim pole a naplnim hodnotami 0-24
	vector<vector<double>> values;
	for (int i = 0; i < rows; i++)
	{
		vector<double> row;
		for (int j = 0; j < cols; j++)
			row.push_back(i*cols + j);
		values.push_back(row);
	}

	//vlozim do matice
	EXPECT_TRUE(matrix->set(values));

	//pridam dalsi radek a vlozim do matice -> nemelo by se vlezt
	vector<double> row{ 25,26,27,28,29 };
	values.push_back(row);
	EXPECT_FALSE(matrix->set(values));

	//vyzkousim par nahodnych hodnot
	EXPECT_EQ(matrix->get(2, 3), 13.0);	 // 3.rad., 4.sl -> 2*5+3 = 13
	EXPECT_EQ(matrix->get(1, 4), 9.0);	 // 2.rad., 5.sl -> 1*5+4 = 9
	EXPECT_EQ(matrix->get(0, 1), 1.0);	 // 1.rad., 2.sl -> 0*5+1 = 1
	EXPECT_EQ(matrix->get(3, 0), 15.0);	 // 4.rad., 1.sl -> 3*5+0 = 15
}
TEST_F(GetSetTest, Getter)
{
	//naplnim matici hodnotami 0-24
	for (int i = 0; i < rows; i++)
		for (int j = 0; j < cols; j++)
			ASSERT_TRUE(matrix->set(i, j, i*cols + j));

	//test mimo meze
	for (int i = rows; i < 2 * rows; i++)
		for (int j = cols; j < 2 * cols; j++)
			EXPECT_ANY_THROW(matrix->get(i, j));

	//test v mezich
	for (int i = 0; i < rows; i++)
		for (int j = 0; j < cols; j++)
			ASSERT_EQ(matrix->get(i, j), i*cols + j);
}

/*testy operatoru*/
//fixture trida
class OperatorsTest : public ::testing::Test
{
protected:
	Matrix matrix;
	int rows = 3, cols = 3;
};

TEST_F(OperatorsTest, EqualOperator)
{
	//cyklus 5 testu nad ruznymi velikostmi a typy matic
	for (int test = 0; test < 5; test++)
	{
		matrix = Matrix(rows, cols);
		//overeni vyjimky pri jine velikosti porovnavane matice
		Matrix unequalMatrix(rows + 1, cols + 1);
		EXPECT_ANY_THROW(matrix == unequalMatrix);

		//overeni stejne matice ... naplnime obe matice 0...rows*cols+1
		Matrix equalMatrix(rows, cols);
		for (int i = 0; i < rows; i++)
			for (int j = 0; j < cols; j++)
			{
				EXPECT_TRUE(matrix.set(i, j, i*cols + j));
				EXPECT_TRUE(equalMatrix.set(i, j, i*cols + j));
			}
		EXPECT_TRUE(matrix == equalMatrix);

		//zmenime prostredni hodnotu treba na 1000 -> nebude stejna
		EXPECT_TRUE(equalMatrix.set(rows / 2, cols / 2, 1000));
		ASSERT_FALSE(matrix == equalMatrix);

		//navysime pocty radku a sloupcu nerovnomerne pro jinaci strukturu
		rows *= 2;
		cols *= 3;
	}
}
TEST_F(OperatorsTest, PlusOperator)
{
	//cyklus 5 testu nad ruznymi velikostmi a typy matic
	for (int test = 0; test < 5; test++)
	{
		matrix = Matrix(rows, cols);
		//overeni vyjimky pri jine velikosti matice na souctu
		Matrix unequalMatrix(rows + 1, cols + 1);
		EXPECT_ANY_THROW(matrix == unequalMatrix);

		//naplnime matice opacnymi hodnotami (0...rows*cols-1, rows*cols-1...0)
		Matrix equalMatrix = Matrix(rows, cols);
		for (int i = 0; i < rows; i++)
			for (int j = 0; j < cols; j++)
			{
				EXPECT_TRUE(matrix.set(i, j, i*cols + j));
				EXPECT_TRUE(equalMatrix.set(i, j, -(i*cols + j)));
			}
		//soucet by mel dat nulovou matici
		Matrix result = matrix + equalMatrix;
		for (int i = 0; i < rows; i++)
			for (int j = 0; j < cols; j++)
				ASSERT_FALSE(result.get(i, j));

		//navysime pocty radku a sloupcu nerovnomerne pro jinaci strukturu
		rows *= 2;
		cols *= 3;
	}
}
TEST_F(OperatorsTest, MatrixMultiplyOperator)
{
	int rows = 2;
	//cyklus 5 testu nad ruznymi velikostmi a typy matic
	for (int test = 0; test < 5; test++)
	{
		//vytvoreni dobre, spatne a hlavni matice
		matrix = Matrix(rows, cols);
		Matrix goodMatrix(cols, rows);
		Matrix badMatrix(rows, cols);

		//naplneni matic hodnotami
		for (int i = 0; i < rows - 1; i++)
			for (int j = 0; j < cols; j++)
			{
				EXPECT_TRUE(matrix.set(i, j, i*cols + j));
				EXPECT_TRUE(badMatrix.set(i, j, i*cols + j));
				EXPECT_TRUE(goodMatrix.set(j, i, j*cols + i));
			}

		//overeni vyjimky, pokud nebudou sloupce jedne rovny radkum druhe a vice versa
		EXPECT_ANY_THROW(matrix*badMatrix);
		EXPECT_NO_THROW(matrix*goodMatrix);

		//vypocet ocekavane matice algoritmem i vysledku operatoru
		Matrix result = matrix * goodMatrix;
		Matrix expected(rows, rows);
		for (int m = 0; m < rows; m++)
		{
			for (int n = 0; n < rows; n++)
			{
				EXPECT_TRUE(expected.set(m, n, 0));
				for (int k = 0; k < cols; k++)
				{
					EXPECT_TRUE(expected.set(m, n, expected.get(m, n) + matrix.get(m, k) * goodMatrix.get(k, n)));
				}
			}
		}
		//vysledek a ocekavana by se mely rovnat
		ASSERT_TRUE(result == expected);
		//navysime pocty radku a sloupcu nerovnomerne pro jinaci strukturu
		rows *= 2;
		cols *= 3;
	}
}
TEST_F(OperatorsTest, ConstMultiplyOperator)
{
	//cyklus 5 testu nad ruznymi velikostmi a typy matic
	for (int test = 0; test < 5; test++)
	{
		matrix = Matrix(rows, cols);
		Matrix expected(rows, cols);
		const double K = 4.0;
		//naplneni matice a ocekavaneho vysledku
		for (int i = 0; i < rows; i++)
			for (int j = 0; j < cols; j++)
			{
				EXPECT_TRUE(matrix.set(i, j, i*cols + j));
				EXPECT_TRUE(expected.set(i, j, K*(i*cols + j)));
			}
		//ocekavana by se mela rovna vysledku
		ASSERT_TRUE(expected == matrix * K);
		//navysime pocty radku a sloupcu nerovnomerne pro jinaci strukturu
		rows *= 2;
		cols *= 3;
	}
}

/*test na reseni rovnice*/
//fixture trida
class EquationTest : public ::testing::Test
{
protected:
	Matrix *matrix;
	vector<double> results;		  //pole vysledku
	int rows = 3;				  //matice musi byt ctvercova -> staci jeden parametr

	virtual void SetUp()
	{
		matrix = new Matrix(rows, rows);
	}
	virtual void TearDown() override
	{
		delete matrix;
	}
};
//test vyvolani vyjimek
TEST_F(EquationTest, ExceptionTest)
{
	//pomocny vektor
	vector<double> values;
	for (int i = 0; i <= rows; i++)
		values.push_back(i);

	//overeni vyjimky na pocet clenu ku poctu radku
	EXPECT_ANY_THROW(matrix->solveEquation(values));

	//overeni vyjimky na singularni matici
	//vyuzijeme toho, ze ma-li 2-vsechny radky/sloupce stejne, je singularni
	values.pop_back();
	for (int i = 0; i < rows; i++)
		for (int j = 0; j < rows; j++)
			EXPECT_TRUE(matrix->set(i, j, (i + 1)*(j + 1)));
	EXPECT_ANY_THROW(matrix->solveEquation(values));

	//overeni vyjimky na ctvercovou matici
	delete matrix;
	matrix = new Matrix(rows, rows + 1);
	EXPECT_ANY_THROW(matrix->solveEquation(values));
}
//test pro homogenni matici -> vysledky musi byt nuly
TEST_F(EquationTest, HomogeneousMatrix)
{
	vector<double> values;
	int matrixInput[3][3] = { {3,8,4},
							  {6,6,5},
							  {1,7,4} };
	for (int i = 0; i < rows; i++)
	{
		values.push_back(0);
		for (int j = 0; j < rows; j++)
			EXPECT_TRUE(matrix->set(i, j, matrixInput[i][j]));
	}
	results = matrix->solveEquation(values);
	for (unsigned int i = 0; i < results.size(); i++)
		ASSERT_FALSE(results[i]);
}
//** 2 testy s priklady, kdy i prava strana ma konkretni hodnoty
//prvni priklad - matice 3.radu
TEST_F(EquationTest, NonhomogeneousMatrix1)
{
	//naplneni matice
	int matrixInput[3][3] = { {2,4,7},
							  {1,5,3},
							  {2,3,5} };
	for (int i = 0; i < rows; i++)
		for (int j = 0; j < rows; j++)
			EXPECT_TRUE(matrix->set(i, j, matrixInput[i][j]));
	//naplneni prave strany
	vector<double> values{ 78,60,58 };
	//vysledek z Matlabu
	vector<double> expected{ 2,8,6 };
	results = matrix->solveEquation(values);
	for (unsigned int i = 0; i < results.size(); i++)
		ASSERT_EQ(results[i], expected[i]);
}
TEST_F(EquationTest, NonhomogeneousMatrix2)
{
	rows = 5;
	delete matrix;
	matrix = new Matrix(rows, rows);
	//naplneni matice
	int matrixInput[5][5] = { {7,6,4,2,3},
							  {4,8,2,1,6},
							  {7,1,5,9,6},
							  {8,2,1,3,4},
							  {6,2,4,9,5} };
	for (int i = 0; i < rows; i++)
		for (int j = 0; j < rows; j++)
			EXPECT_TRUE(matrix->set(i, j, matrixInput[i][j]));
	//naplneni prave strany
	vector<double> values{ 92,100,134,81,131 };
	//vysledek z Matlabu
	vector<double> expected{ 3,6,1,8,5 };
	results = matrix->solveEquation(values);
	for (unsigned int i = 0; i < results.size(); i++)
		ASSERT_EQ(results[i], expected[i]);
}

/*** Konec souboru white_box_tests.cpp ***/