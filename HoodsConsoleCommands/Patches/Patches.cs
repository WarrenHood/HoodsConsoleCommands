using System;
using System.Collections.Generic;
using System.Text;
using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;
using System.Linq;

namespace HoodsConsoleCommands.Patches {
    public class Patches {
        
        static List<String> GetArgs(String command) {
            List<String> args = new List<string>();
            String current = "";
            bool inString = false;
            char currentQuote = '"';
            bool escapeNext = false;

            foreach (char c in command) {
                // Escape the current char
                if (escapeNext) {
                    current += c;
                    escapeNext = false;
                    continue;
                }

                // We encounter an escape
                if (c == '\\') {
                    escapeNext = true;
                    continue;
                }

                if (inString) {
                    // Currently in a string
                    
                    // End of a string
                    if (c == currentQuote && !escapeNext) {
                        inString = false;
                        args.Add(current);
                        current = "";
                        continue;
                    }

                    // Otherwise this is normal stuff in the string
                    current += c;
                }
                else {
                    // Not in a string

                    // Split args at spaces
                    if (c == ' ' || c == '\t') {
                        args.Add(current);
                        current = "";
                        continue;
                    }

                    // Beginning of a string
                    if (c == '"' || c == '\'') {
                        args.Add(current);
                        current = "";
                        inString = true;
                        currentQuote = c;
                        continue;
                    }

                    // Anything else
                    current += c;
                }
            }
            if (current.Trim().Length > 0) {
                args.Add(current.Trim());
            }

            return args;
        }

        [HarmonyPatch(typeof(HUDManager), "SubmitChat_performed")]
        [HarmonyPrefix]
        static void OnChatMessage(HUDManager __instance) {
            if (__instance.localPlayer == null || !__instance.localPlayer.isTypingChat) {
                return;
            }

            if ((!__instance.localPlayer.IsOwner || (__instance.IsServer && !__instance.localPlayer.isHostPlayerObject)) && !__instance.localPlayer.isTestingPlayer) {
                return;
            }

            RoundManager roundManager = GameObject.FindObjectOfType<RoundManager>();

            // Ensure that the player is the host
            if (roundManager == null || !roundManager.IsHost) {
                return;
            }

            String chatMessage = __instance.chatTextField.text.Trim();

            __instance.chatTextField.text = "";

            // All commands should start with a /
            if (!chatMessage.StartsWith("/")) {
                return;
            }
            List<String> command = GetArgs(chatMessage.Substring(1));

            if (command.Count == 0) {
                HUDManager.Instance.AddTextToChatOnServer("Empty command...");
                return;
            }

            if (command[0] == "help") {
                HUDManager.Instance.AddTextToChatOnServer("Available commands\n" +
                    "Help: Shows this help"
                );
            }
        }
    }
}
