using Project.Scripts.NodeSystem.Quests.Nodes;

namespace Project.Scripts.NodeSystem.Quests
{
    public class DialogueTuple
    {
        public DialogueGraph dialogueGraph;
        public QuestGraphProcessor questGraphProcessor;

        public DialogueTuple(DialogueGraph dialogueGraph, QuestGraphProcessor questGraphProcessor)
        {
            this.dialogueGraph = dialogueGraph;
            this.questGraphProcessor = questGraphProcessor;
        }
    }
}