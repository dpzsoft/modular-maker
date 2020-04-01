using System;
using System.Collections.Generic;
using System.Text;

namespace controller {
    internal class Template {

        private enum ParseType {
            Normal = 0x00,
            Define = 0x01,
            PartCommand = 0x11,
            PartContent = 0x12
        }

        /// <summary>
        /// 从字符串模板定义中加载模板结果
        /// </summary>
        /// <param name="str"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public static string FromPartString(string str, dpz3.Json.JsonUnit package, dpz3.Xml.XmlNode table, dpz3.Xml.XmlNode field) {

            // 字符串创建器
            StringBuilder sb = new StringBuilder();

            // 命令解析器
            ParseType pt = ParseType.Normal;
            StringBuilder sbCmd = new StringBuilder();

            for (int i = 0; i < str.Length; i++) {
                char chr = str[i];

                switch (chr) {
                    case '$':
                        if (pt == ParseType.Define) {
                            sb.Append(chr);
                            pt = ParseType.Normal;
                        } else {
                            pt = ParseType.Define;
                        }
                        break;
                    case '{':
                        if (pt == ParseType.Define) {
                            if (sbCmd.Length > 0) throw new Exception("语法错误");
                            sbCmd.Append(chr);
                        } else {
                            sb.Append(chr);
                        }
                        break;
                    case '}':
                        if (pt == ParseType.Define) {
                            if (sbCmd.Length <= 0) throw new Exception("语法错误");
                            if (sbCmd[0] != '{') throw new Exception("语法错误");

                            // 获取定义内容
                            sbCmd.Remove(0, 1);
                            string cmdStr = sbCmd.ToString();
                            sbCmd.Clear();

                            string[] cmds = cmdStr.Split('.');
                            if (cmds.Length != 2) throw new Exception($"不支持的对象\"{cmdStr}\"");

                            // 判断命令类型
                            switch (cmds[0]) {
                                case "package":
                                    sb.Append(package.Str[cmds[1]]);
                                    break;
                                case "table":
                                    sb.Append(table.Attr[cmds[1]]);
                                    break;
                                case "field":
                                    if (cmds[1] == "data") {
                                        sb.Append(field["data"].Attr[cmds[2]]);
                                    } else {
                                        sb.Append(field.Attr[cmds[1]]);
                                    }
                                    //sb.Append(field.GetType().GetProperty(cmds[1]).GetValue(field));
                                    break;
                                default: throw new Exception($"不支持的对象\"{cmdStr}\"");
                            }

                            // 重置解析模式
                            pt = ParseType.Normal;
                        } else {
                            sb.Append(chr);
                        }
                        break;
                    default:
                        if (pt == ParseType.Define) {
                            sbCmd.Append(chr);
                        } else {
                            sb.Append(chr);
                        }
                        break;
                }

            }

            return sb.ToString();

        }

        /// <summary>
        /// 从字符串模板定义中加载模板结果
        /// </summary>
        /// <param name="str"></param>
        /// <param name="table"></param>
        /// <returns></returns>
        public static string FromString(string str, dpz3.Json.JsonUnit package, dpz3.Xml.XmlNode table) {

            // 字符串创建器
            StringBuilder sb = new StringBuilder();

            // 命令解析器
            ParseType pt = ParseType.Normal;
            StringBuilder sbCmd = new StringBuilder();
            StringBuilder sbPart = new StringBuilder();

            for (int i = 0; i < str.Length; i++) {
                char chr = str[i];

                switch (chr) {
                    case '$':
                        if (pt == ParseType.Define) {
                            sb.Append(chr);
                            pt = ParseType.Normal;
                        } else if (pt == ParseType.PartCommand) {
                            throw new Exception("语法错误");
                        } else if (pt == ParseType.PartContent) {
                            sbPart.Append(chr);
                        } else {
                            pt = ParseType.Define;
                        }
                        break;
                    case '@':
                        #region [=====解析段定义=====]
                        if (pt == ParseType.PartCommand) {
                            if (sbCmd.Length <= 0) {
                                // @符号转义
                                sb.Append(chr);
                                pt = ParseType.Normal;
                            } else {
                                throw new Exception("语法错误");
                            }
                        } else if (pt == ParseType.PartContent) {
                            sbPart.Append(chr);
                        } else if (pt == ParseType.Define) {
                            throw new Exception("语法错误");
                        } else {
                            pt = ParseType.PartCommand;
                        }
                        break;
                    #endregion
                    case ';':
                        #region [=====解析段定义=====]
                        if (pt == ParseType.PartCommand) {
                            throw new Exception("语法错误");
                        } else if (pt == ParseType.PartContent) {

                            if (sbPart.Length <= 0) throw new Exception("语法错误");

                            // 判断是否为端结束
                            if (sbPart[sbPart.Length - 1] == '}') {

                                // 段定义结束
                                sbPart.Remove(sbPart.Length - 1, 1);
                                string part = sbPart.ToString();
                                sbPart.Clear();

                                // 获取脚本内容
                                string cmdStr = sbCmd.ToString();
                                sbCmd.Clear();

                                #region [=====代码段执行=====]
                                // 命令模式
                                if (cmdStr.StartsWith("(") && cmdStr.EndsWith(")")) {
                                    // 命令采用都号隔开
                                    string[] cmds = cmdStr.Split(',');
                                    // 解析命令
                                    switch (cmds[0].Trim()) {
                                        case "list":
                                            // 列表模式
                                            if (cmds.Length != 2) throw new Exception("语法错误");
                                            string[] varParts = cmds[1].Trim().Split('.');
                                            if (varParts.Length != 2) throw new Exception("语法错误");
                                            switch (varParts[0]) {
                                                case "table":
                                                    if (varParts[1] == "Fields") {
                                                        foreach (var field in table.GetNodesByTagName("field", false)) {
                                                            sb.Append(FromPartString(part, package, table, field));
                                                        }
                                                    } else {
                                                        throw new Exception($"不支持的对象\"{cmds[1].Trim()}\"");
                                                    }
                                                    break;
                                                default: throw new Exception($"不支持的对象\"{cmds[1].Trim()}\"");
                                            }
                                            break;
                                    }
                                } else {
                                    // 直接定义内容则为默认的list段
                                    string[] varParts = cmdStr.Trim().Split('.');
                                    if (varParts.Length != 2) throw new Exception("语法错误");
                                    switch (varParts[0]) {
                                        case "table":
                                            if (varParts[1] == "Fields") {
                                                foreach (var field in table.GetNodesByTagName("field", false)) {
                                                    sb.Append(FromPartString(part, package, table, field));
                                                }
                                            } else {
                                                throw new Exception($"不支持的对象\"{cmdStr.Trim()}\"");
                                            }
                                            break;
                                        default: throw new Exception($"不支持的对象\"{cmdStr.Trim()}\"");
                                    }
                                }
                                #endregion

                                // 重新设置解析模式
                                pt = ParseType.Normal;
                            } else if (sbPart[sbPart.Length - 1] == '$') {
                                // 转义模式
                                sbPart[sbPart.Length - 1] = chr;
                            } else {
                                sbPart.Append(chr);
                            }

                        } else if (pt == ParseType.Define) {
                            throw new Exception("语法错误");
                        } else {
                            sb.Append(chr);
                        }
                        break;
                    #endregion
                    case '{':
                        if (pt == ParseType.Define) {
                            if (sbCmd.Length > 0) throw new Exception("语法错误");
                            sbCmd.Append(chr);
                        } else if (pt == ParseType.PartCommand) {
                            pt = ParseType.PartContent;
                        } else if (pt == ParseType.PartContent) {
                            sbPart.Append(chr);
                        } else {
                            sb.Append(chr);
                        }
                        break;
                    case '}':
                        if (pt == ParseType.Define) {
                            if (sbCmd.Length <= 0) throw new Exception("语法错误");
                            if (sbCmd[0] != '{') throw new Exception("语法错误");

                            // 获取定义内容
                            sbCmd.Remove(0, 1);
                            string cmdStr = sbCmd.ToString();
                            sbCmd.Clear();

                            string[] cmds = cmdStr.Split('.');
                            if (cmds.Length != 2) throw new Exception($"不支持的对象\"{cmdStr}\"");

                            // 判断命令类型
                            switch (cmds[0]) {
                                case "package":
                                    sb.Append(package.Str[cmds[1]]);
                                    break;
                                case "table":
                                    sb.Append(table.Attr[cmds[1]]);
                                    break;
                                default: throw new Exception($"不支持的对象\"{cmdStr}\"");
                            }

                            // 设置解析模式
                            pt = ParseType.Normal;
                        } else if (pt == ParseType.PartCommand) {
                            throw new Exception("语法错误");
                        } else if (pt == ParseType.PartContent) {
                            sbPart.Append(chr);
                        } else {
                            sb.Append(chr);
                        }
                        break;
                    default:
                        if (pt == ParseType.Define || pt == ParseType.PartCommand) {
                            sbCmd.Append(chr);
                        } else if (pt == ParseType.PartContent) {
                            sbPart.Append(chr);
                        } else {
                            sb.Append(chr);
                        }
                        break;
                }

            }

            return sb.ToString();

        }


    }
}
