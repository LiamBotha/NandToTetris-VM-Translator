using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;

namespace VMTranslator
{
    class Program
    {
        static int boolIndex = 0;

        static int Main(string[] args)
        {
            string filepath = Console.ReadLine();

            while(!File.Exists(filepath))
            {
                Console.WriteLine("File Not Found");
                filepath = Console.ReadLine();
            }

            List<String> linesToTranslate = new List<string>();

            using (StreamReader reader = new StreamReader(filepath))
            {
                string line = "";
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.StartsWith("//") || line == "")
                        continue;

                    Console.WriteLine("// " + line);

                    linesToTranslate.Add(line);
                }
            }

            using (StreamWriter writer = new StreamWriter(filepath.Replace("vm", "asm")))
            {
                for (int i = 0; i < linesToTranslate.Count; i++)
                {
                    //Creates comment in for each vm command for debugging
                    writer.WriteLine("//" + linesToTranslate[i]);

                    //Parse Line to Commands
                    var type = CommandType(linesToTranslate[i]);

                    //Translate to Assembly and Write to File
                    if (type == COMMAND_TYPE.C_ARITHMETIC)
                    {
                        WriteAritmetic(writer, linesToTranslate[i]);
                    }
                    else if (type == COMMAND_TYPE.C_PUSH || type == COMMAND_TYPE.C_POP)
                    {
                        WritePushPop(writer, type, linesToTranslate[i], Path.GetFileNameWithoutExtension(filepath));
                    }
                }
            }

            return 0;
        }

        private static void WritePushPop(StreamWriter writer, COMMAND_TYPE type, string line, string filename)
        {
            var args = line.Split(" ");

            if (type == COMMAND_TYPE.C_PUSH)
            {
                //push local 4

                if (args[1] == "constant")
                {
                    //Get i
                    writer.WriteLine("@" + args[2]);
                    writer.WriteLine("D=A");
                }
                else if(args[1] == "static")
                {
                    //Get value at foo.i
                    writer.WriteLine("@" + filename + "." + args[2]);
                    writer.WriteLine("D=M");
                }
                else if (args[1] == "local")
                {
                    //Get i
                    writer.WriteLine("@" + args[2]);
                    writer.WriteLine("D=A");

                    writer.WriteLine("@LCL");
                    writer.WriteLine("A=M+D");
                    writer.WriteLine("D=M");
                }
                else if (args[1] == "argument")
                {
                    //Get i
                    writer.WriteLine("@" + args[2]);
                    writer.WriteLine("D=A");

                    writer.WriteLine("@ARG");
                    writer.WriteLine("A=M+D");
                    writer.WriteLine("D=M");

                }
                else if (args[1] == "this")
                {
                    //Get i
                    writer.WriteLine("@" + args[2]);
                    writer.WriteLine("D=A");

                    writer.WriteLine("@THIS");
                    writer.WriteLine("A=M+D");
                    writer.WriteLine("D=M");
                }
                else if (args[1] == "that")
                {
                    //Get i
                    writer.WriteLine("@" + args[2]);
                    writer.WriteLine("D=A");

                    writer.WriteLine("@THAT");
                    writer.WriteLine("A=M+D");
                    writer.WriteLine("D=M");
                }
                else if (args[1] == "pointer")
                {
                    //Get i
                    writer.WriteLine("@" + args[2]);
                    writer.WriteLine("D=A");

                    writer.WriteLine("@3");
                    writer.WriteLine("A=A+D");
                    writer.WriteLine("D=M");
                }
                else if (args[1] == "temp")
                {
                    //Get i
                    writer.WriteLine("@" + args[2]);
                    writer.WriteLine("D=A");

                    writer.WriteLine("@5");
                    writer.WriteLine("A=A+D");
                    writer.WriteLine("D=M");
                }

                //ADD TO Stack
                writer.WriteLine("@SP");
                writer.WriteLine("A=M");
                writer.WriteLine("M=D");

                //Increment *SP
                writer.WriteLine("@SP");
                writer.WriteLine("M=M+1");
            }
            else if(type == COMMAND_TYPE.C_POP)
            {
                if (args[1] == "constant")
                {
                    return;
                }

                //Pop Local 3
                if(args[1] == "static")
                {
                    writer.WriteLine("@SP");
                    writer.WriteLine("AM=M-1");
                    writer.WriteLine("D=M");
                    writer.WriteLine("@" + filename + "." + args[2]);
                    writer.WriteLine("M=D");
                }
                else if (args[1] == "local")
                {
                    writer.WriteLine("@" + args[2]);
                    writer.WriteLine("D=A");
                    writer.WriteLine("@LCL");
                    writer.WriteLine("D=M+D");
                    writer.WriteLine("@R13");
                    writer.WriteLine("M=D");

                    writer.WriteLine("@SP");
                    writer.WriteLine("AM=M-1");
                    writer.WriteLine("D=M");

                    writer.WriteLine("@R13");
                    writer.WriteLine("A=M");
                    writer.WriteLine("M=D");
                }
                else if (args[1] == "argument")
                {
                    writer.WriteLine("@" + args[2]);
                    writer.WriteLine("D=A");
                    writer.WriteLine("@ARG");
                    writer.WriteLine("D=M+D");
                    writer.WriteLine("@R13");
                    writer.WriteLine("M=D");

                    writer.WriteLine("@SP");
                    writer.WriteLine("AM=M-1");
                    writer.WriteLine("D=M");

                    writer.WriteLine("@R13");
                    writer.WriteLine("A=M");
                    writer.WriteLine("M=D");
                }
                else if (args[1] == "this")
                {
                    writer.WriteLine("@" + args[2]);
                    writer.WriteLine("D=A");
                    writer.WriteLine("@THIS");
                    writer.WriteLine("D=M+D");
                    writer.WriteLine("@R13");
                    writer.WriteLine("M=D");

                    writer.WriteLine("@SP");
                    writer.WriteLine("AM=M-1");
                    writer.WriteLine("D=M");

                    writer.WriteLine("@R13");
                    writer.WriteLine("A=M");
                    writer.WriteLine("M=D");
                }
                else if (args[1] == "that")
                {
                    writer.WriteLine("@" + args[2]);
                    writer.WriteLine("D=A");
                    writer.WriteLine("@THAT");
                    writer.WriteLine("D=M+D");
                    writer.WriteLine("@R13");
                    writer.WriteLine("M=D");

                    writer.WriteLine("@SP");
                    writer.WriteLine("AM=M-1");
                    writer.WriteLine("D=M");

                    writer.WriteLine("@R13");
                    writer.WriteLine("A=M");
                    writer.WriteLine("M=D");
                }
                else if (args[1] == "pointer")
                {
                    writer.WriteLine("@" + args[2]);
                    writer.WriteLine("D=A");
                    writer.WriteLine("@3");
                    writer.WriteLine("D=A+D");
                    writer.WriteLine("@R13");
                    writer.WriteLine("M=D");

                    writer.WriteLine("@SP");
                    writer.WriteLine("AM=M-1");
                    writer.WriteLine("D=M");

                    writer.WriteLine("@R13");
                    writer.WriteLine("A=M");
                    writer.WriteLine("M=D");
                }
                else if (args[1] == "temp")
                {
                    writer.WriteLine("@" + args[2]);
                    writer.WriteLine("D=A");
                    writer.WriteLine("@5");
                    writer.WriteLine("D=A+D");
                    writer.WriteLine("@R13");
                    writer.WriteLine("M=D");

                    writer.WriteLine("@SP");
                    writer.WriteLine("AM=M-1");
                    writer.WriteLine("D=M");

                    writer.WriteLine("@R13");
                    writer.WriteLine("A=M");
                    writer.WriteLine("M=D");
                }
            }
        }

        private static void WriteAritmetic(StreamWriter writer, string line)
        {
            // need to reset values to 0
            string trueLabel = "t_" + boolIndex;
            string falseLabel = "f_" + boolIndex;

            if (line.StartsWith("add"))
            {
                //*SP--
                writer.WriteLine("@SP");
                writer.WriteLine("AM=M-1");

                //a+b
                writer.WriteLine("D=M");
                writer.WriteLine("A=A-1");
                writer.WriteLine("M=M+D");
            }
            else if(line.StartsWith("sub"))
            {
                //*SP--
                writer.WriteLine("@SP");
                writer.WriteLine("AM=M-1");

                //a-b
                writer.WriteLine("D=M");
                writer.WriteLine("A=A-1");
                writer.WriteLine("M=M-D");
            }
            else if(line == "neg")
            {
                writer.WriteLine("@SP");
                writer.WriteLine("A=M-1");
                writer.WriteLine("M=-M");
            }
            else if(line == "eq")
            {
                //*SP-- & A = *Sp
                writer.WriteLine("@SP");
                writer.WriteLine("AM=M-1");

                // A - B
                writer.WriteLine("D=M");
                writer.WriteLine("A=A-1");
                writer.WriteLine("D=D-M");

                // if( == 0) goto true
                writer.WriteLine("@" + trueLabel);
                writer.WriteLine("D;JEQ");

                // goto false
                writer.WriteLine("@" + falseLabel);
                writer.WriteLine("0;JMP");

                //(True) set to -1
                writer.WriteLine("(" + trueLabel + ")");
                writer.WriteLine("@SP");
                writer.WriteLine("A=M-1");
                writer.WriteLine("M=-1");

                //goto End
                writer.WriteLine("@EndEq_" + boolIndex);
                writer.WriteLine("0;JMP");

                //(false) set to 0
                writer.WriteLine("(" + falseLabel + ")");
                writer.WriteLine("@SP");
                writer.WriteLine("A=M-1");
                writer.WriteLine("M=0");

                //(End)
                writer.WriteLine("(EndEq_" + boolIndex + ")");

                boolIndex++;
            }
            else if (line == "gt")
            {
                //*SP-- & A = *Sp
                writer.WriteLine("@SP");
                writer.WriteLine("AM=M-1");

                // A - B
                writer.WriteLine("D=M");
                writer.WriteLine("A=A-1");
                writer.WriteLine("D=D-M");

                // if( > 0) goto true
                writer.WriteLine("@" + trueLabel);
                writer.WriteLine("D;JLT");

                // goto false
                writer.WriteLine("@" + falseLabel);
                writer.WriteLine("0;JMP");

                //(True) set to -1
                writer.WriteLine("(" + trueLabel + ")");
                writer.WriteLine("@SP");
                writer.WriteLine("A=M-1");
                writer.WriteLine("M=-1");

                //goto End
                writer.WriteLine("@EndGt_" + boolIndex);
                writer.WriteLine("0;JMP");

                //(false) set to 0
                writer.WriteLine("(" + falseLabel + ")");
                writer.WriteLine("@SP");
                writer.WriteLine("A=M-1");
                writer.WriteLine("M=0");

                //(End)
                writer.WriteLine("(EndGt_" + boolIndex + ")");

                boolIndex++;
            }
            else if (line == "lt")
            {
                //*SP-- & A = *Sp
                writer.WriteLine("@SP");
                writer.WriteLine("AM=M-1");

                // A - B
                writer.WriteLine("D=M");
                writer.WriteLine("A=A-1");
                writer.WriteLine("D=D-M");

                // if( < 0) goto true
                writer.WriteLine("@" + trueLabel);
                writer.WriteLine("D;JGT");

                // goto false
                writer.WriteLine("@" + falseLabel);
                writer.WriteLine("0;JMP");

                //(True) set to -1
                writer.WriteLine("(" + trueLabel + ")");
                writer.WriteLine("@SP");
                writer.WriteLine("A=M-1");
                writer.WriteLine("M=-1");

                //goto End
                writer.WriteLine("@EndLt_" + boolIndex);
                writer.WriteLine("0;JMP");

                //(false) set to 0
                writer.WriteLine("(" + falseLabel + ")");
                writer.WriteLine("@SP");
                writer.WriteLine("A=M-1");
                writer.WriteLine("M=0");

                //(End)
                writer.WriteLine("(EndLt_" + boolIndex + ")");

                boolIndex++;
            }
            else if (line == "and")
            {
                //Set address to top of stack
                writer.WriteLine("@SP");
                writer.WriteLine("AM=M-1");

                // M = Binary A & B
                writer.WriteLine("D=M");
                writer.WriteLine("A=A-1");
                writer.WriteLine("M=D&M");
            }
            else if (line == "or")
            {
                //Set address to top of stack
                writer.WriteLine("@SP");
                writer.WriteLine("AM=M-1");

                // M = Binary A | B
                writer.WriteLine("D=M");
                writer.WriteLine("A=A-1");
                writer.WriteLine("M=D|M");
            }
            else if (line == "not")
            {
                // Set Y to -Y
                writer.WriteLine("@SP");
                writer.WriteLine("A=M-1");
                writer.WriteLine("M=!M");
            }
        }

        private static COMMAND_TYPE CommandType(string line)
        {
            if(line.StartsWith("add") || line.StartsWith("sub") || line.StartsWith("neg") 
                || line.StartsWith("eq") || line.StartsWith("gt") || line.StartsWith("lt") 
                || line.StartsWith("and") || line.StartsWith("or") || line.StartsWith("not"))
            {
                return COMMAND_TYPE.C_ARITHMETIC;
            }
            else if (line.StartsWith("push"))
            {
                return COMMAND_TYPE.C_PUSH;
            }
            else if (line.StartsWith("pop"))
            {
                return COMMAND_TYPE.C_POP;
            }

            return 0;
        }
    }

    enum COMMAND_TYPE
    {
        C_NULL = 0,
        C_ARITHMETIC,
        C_PUSH,
        C_POP,
        C_LABEL,
        C_GOTO,
        C_IF,
        C_FUCTION,
        C_RETURN,
        C_CALL
    }
}
