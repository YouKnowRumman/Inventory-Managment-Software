
import React, { useState, useEffect, useRef } from 'react';
import { Send, User as UserIcon, Clock, MessageSquare } from 'lucide-react';
import { Post, User } from '../types.js';
import { motion, AnimatePresence } from 'framer-motion';

interface ChatRoomProps {
  inventoryId: string;
  user: User | null;
}

const ChatRoom: React.FC<ChatRoomProps> = ({ inventoryId, user }) => {
  const [posts, setPosts] = useState<Post[]>([]);
  const [newPost, setNewPost] = useState('');
  const scrollRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    const fetchPosts = () => {
      const mockPosts: Post[] = [
        { id: '1', inventoryId, userId: '2', userName: 'Rahim Ali', content: 'Verified entry. Data looks accurate.', timestamp: Date.now() - 3600000 },
        { id: '2', inventoryId, userId: '3', userName: 'Suhasini Dey', content: 'Updates received. Processing final review.', timestamp: Date.now() - 1800000 },
      ];
      setPosts(mockPosts);
    };

    fetchPosts();
    const interval = setInterval(fetchPosts, 4500);
    return () => clearInterval(interval);
  }, [inventoryId]);

  useEffect(() => {
    if (scrollRef.current) {
      scrollRef.current.scrollTop = scrollRef.current.scrollHeight;
    }
  }, [posts]);

  const handleSend = () => {
    if (!newPost.trim() || !user) return;
    const post: Post = {
      id: Math.random().toString(),
      inventoryId,
      userId: user.id,
      userName: user.name,
      content: newPost,
      timestamp: Date.now(),
    };
    setPosts(prev => [...prev, post]);
    setNewPost('');
  };

  return (
    <div className="flex flex-col h-[550px] bg-executive-surface dark:bg-command-surface rounded-lg border border-executive-border dark:border-command-elevated shadow-executive dark:shadow-none overflow-hidden transition-theme">
      <div className="px-6 py-4 bg-executive-alt dark:bg-command-elevated border-b border-executive-border dark:border-command-surface flex items-center">
        <MessageSquare className="w-4 h-4 mr-3 text-executive-brand dark:text-command-brand" />
        <h3 className="text-xs font-black uppercase tracking-widest">Operational Log</h3>
      </div>

      <div 
        ref={scrollRef}
        className="flex-1 overflow-y-auto p-6 space-y-6 bg-executive-bg/30 dark:bg-command-bg/30 custom-scrollbar"
      >
        <AnimatePresence initial={false}>
          {posts.map((post) => (
            <motion.div 
              key={post.id} 
              initial={{ opacity: 0, x: -10 }} 
              animate={{ opacity: 1, x: 0 }} 
              className="flex items-start space-x-4"
            >
              <div className="w-9 h-9 rounded-lg bg-executive-alt dark:bg-command-elevated border border-executive-border dark:border-command-surface flex items-center justify-center font-black text-xs text-executive-textSecondary dark:text-command-textSecondary shrink-0">
                {post.userName[0]}
              </div>
              <div className="flex-1 space-y-1.5">
                <div className="flex items-center justify-between">
                  <span className="text-[11px] font-black uppercase tracking-tight text-executive-textPrimary dark:text-command-textPrimary">{post.userName}</span>
                  <span className="text-[9px] text-gray-400 font-bold uppercase flex items-center">
                    <Clock className="w-3 h-3 mr-1" /> {new Date(post.timestamp).toLocaleTimeString()}
                  </span>
                </div>
                <div className="bg-executive-surface dark:bg-command-elevated p-3 rounded rounded-tl-none border border-executive-border dark:border-command-surface text-sm font-medium leading-relaxed">
                  {post.content}
                </div>
              </div>
            </motion.div>
          ))}
        </AnimatePresence>
      </div>

      <div className="p-4 bg-executive-alt dark:bg-command-elevated border-t border-executive-border dark:border-command-surface">
        <div className="flex items-end space-x-2">
          <textarea
            value={newPost}
            onChange={(e) => setNewPost(e.target.value)}
            onKeyDown={(e) => { if (e.key === 'Enter' && !e.shiftKey) { e.preventDefault(); handleSend(); } }}
            placeholder={user ? "Enter operational log entry..." : "Log in to post updates"}
            disabled={!user}
            className="flex-1 p-3 bg-executive-surface dark:bg-command-surface border border-executive-border dark:border-command-surface rounded text-sm font-medium focus:border-executive-brand dark:focus:border-command-brand outline-none resize-none min-h-[48px] max-h-[120px] transition-all disabled:opacity-50"
          />
          <button
            onClick={handleSend}
            disabled={!newPost.trim() || !user}
            className="p-3 bg-executive-brand dark:bg-command-brand text-white rounded border border-transparent hover:brightness-110 disabled:opacity-30 transition-all shadow-executive dark:shadow-none"
          >
            <Send className="w-4 h-4" />
          </button>
        </div>
      </div>
    </div>
  );
};

export default ChatRoom;
